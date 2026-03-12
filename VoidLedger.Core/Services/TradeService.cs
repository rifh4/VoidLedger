using VoidLedger.Core.Services;
using VoidLedger.Core.Stores;

namespace VoidLedger.Core;

public sealed class TradeService : ITradeService
{
    private readonly ILedgerStore _ledgerStore;
    private readonly IClock _clock;

    public TradeService(ILedgerStore ledgerStore, IClock clock)
    {
        _ledgerStore = ledgerStore;
        _clock = clock;
    }

    // Build and validate the full buy outcome before mutating persisted state.
    // This keeps the write path simple and ensures BuyAsync only performs writes for a valid trade.
    private TradeResult BuildBuyResult(string name, int qty, decimal? maybeUnitPrice, decimal currentBalance,
    int currentHoldingQuantity)
    {
        string cleanName = (name ?? "").Trim().ToUpperInvariant();

        if (cleanName.Length == 0)
            return new TradeResult(false, ErrorCode.InvalidName, "Name cannot be empty", null, null, null, null, null);

        if (qty <= 0)
            return new TradeResult(false, ErrorCode.InvalidAmount, "Quantity must be above 0", null, null, null, null, null);

        if (maybeUnitPrice is null)
            return new TradeResult(false, ErrorCode.MissingPrice, "Price not set", null, null, null, null, null);

        decimal unitPrice = maybeUnitPrice.Value;
        decimal total = unitPrice * qty;

        if (total > currentBalance)
            return new TradeResult(false, ErrorCode.InsufficientFunds, "Insufficient funds", null, null, null, null, null);

        decimal newBalance = currentBalance - total;
        int newHoldingQuantity = currentHoldingQuantity + qty;

        string message = $"Bought {qty} {cleanName} for {Formatter.Money(total)}. Balance: {Formatter.Money(newBalance)}";

        return new TradeResult(
            true,
            ErrorCode.None,
            message,
            cleanName,
            unitPrice,
            total,
            newBalance,
            newHoldingQuantity);
    }

    public async Task<OpResult> BuyAsync(string name, int qty)
    {
        string cleanName = (name ?? "").Trim().ToUpperInvariant();

        PriceSnapshot? maybePriceSnapshot = await _ledgerStore.GetPriceAsync(cleanName);
        decimal? maybeUnitPrice = maybePriceSnapshot?.Price;
        decimal currentBalance = await _ledgerStore.GetBalanceAsync();
        int currentHoldingQuantity = await _ledgerStore.GetHoldingQuantityAsync(cleanName) ?? 0;

        TradeResult result = BuildBuyResult(
            cleanName,
            qty,
            maybeUnitPrice,
            currentBalance,
            currentHoldingQuantity);

        if (!result.Ok)
        {
            return new OpResult(false, result.Code, result.Message, null);
        }

        // Persist the balance/holding state change first, then append the audit record,
        // then flush once so the successful trade and its log are saved together.
        await _ledgerStore.SetBalanceAsync(result.NewBalance!.Value);
        await _ledgerStore.SetHoldingQuantityAsync(result.Name!, result.NewHoldingQuantity!.Value);
        

        ActionRecordBase rec = new BuyAction(
            result.Name!,
            qty,
            result.UnitPrice!.Value,
            result.Total!.Value,
            _clock.UtcNow);

        await _ledgerStore.AddActionAsync(rec);
        await _ledgerStore.SaveChangesAsync();
        return new OpResult(true, ErrorCode.None, result.Message, rec);
    }

    // Sell validation is resolved completely before writes so oversell/missing-price/missing-holding
    // cases fail without partially mutating persisted balance or holdings.
    private TradeResult BuildSellResult(
    string name,
    int qty,
    decimal? maybeUnitPrice,
    decimal currentBalance,
    int? currentHoldingQuantity)
    {
        string cleanName = (name ?? "").Trim().ToUpperInvariant();

        if (cleanName.Length == 0)
            return new TradeResult(false, ErrorCode.InvalidName, "Name cannot be empty", null, null, null, null, null);

        if (qty <= 0)
            return new TradeResult(false, ErrorCode.InvalidAmount, "Quantity must be above 0", null, null, null, null, null);

        if (maybeUnitPrice is null)
            return new TradeResult(false, ErrorCode.MissingPrice, "Price not set", null, null, null, null, null);

        if (currentHoldingQuantity is null)
            return new TradeResult(false, ErrorCode.MissingHolding, "No holdings for this name", null, null, null, null, null);

        if (qty > currentHoldingQuantity.Value)
            return new TradeResult(false, ErrorCode.Oversell, "Cannot sell more than you own", null, null, null, null, null);

        decimal unitPrice = maybeUnitPrice.Value;
        decimal total = unitPrice * qty;
        decimal newBalance = currentBalance + total;
        int newHoldingQuantity = currentHoldingQuantity.Value - qty;

        string message = $"Sold {qty} {cleanName} for {Formatter.Money(total)}. Balance: {Formatter.Money(newBalance)}";

        return new TradeResult(
            true,
            ErrorCode.None,
            message,
            cleanName,
            unitPrice,
            total,
            newBalance,
            newHoldingQuantity);
    }

    public async Task<OpResult> SellAsync(string name, int qty)
    {
        string cleanName = (name ?? "").Trim().ToUpperInvariant();

        PriceSnapshot? maybePriceSnapshot = await _ledgerStore.GetPriceAsync(cleanName);
        decimal? maybeUnitPrice = maybePriceSnapshot?.Price; 
        decimal currentBalance = await _ledgerStore.GetBalanceAsync();
        int? currentHoldingQuantity = await _ledgerStore.GetHoldingQuantityAsync(cleanName);

        TradeResult result = BuildSellResult(
            cleanName,
            qty,
            maybeUnitPrice,
            currentBalance,
            currentHoldingQuantity);

        if (!result.Ok)
        {
            return new OpResult(false, result.Code, result.Message, null);
        }

        // Same write order as BuyAsync: apply state changes first, then append the audit record,
        // then save once so the persisted trade outcome and its log stay aligned.
        await _ledgerStore.SetBalanceAsync(result.NewBalance!.Value);
        await _ledgerStore.SetHoldingQuantityAsync(result.Name!, result.NewHoldingQuantity!.Value);
        

        ActionRecordBase rec = new SellAction(
            result.Name!,
            qty,
            result.UnitPrice!.Value,
            result.Total!.Value,
            _clock.UtcNow);

        await _ledgerStore.AddActionAsync(rec);
        await _ledgerStore.SaveChangesAsync();
        return new OpResult(true, ErrorCode.None, result.Message, rec);
    }
}