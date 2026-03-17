using System.Text;
using VoidLedger.Core.Services;
using VoidLedger.Core.Services.Models;
using VoidLedger.Core.Stores;
using VoidLedger.Core.Utilities;

namespace VoidLedger.Core
{
    public class LedgerService : ILedgerService
    {
        private readonly ITradeService _tradeService;
        private readonly IClock _clock;

        // Async persistence boundary. The EF-backed implementation lives in the API project.
        private readonly ILedgerStore _ledgerStore;

        public LedgerService(ITradeService tradeService, IClock clock, ILedgerStore ledgerStore)
        {
            _tradeService = tradeService;
            _clock = clock;
            _ledgerStore = ledgerStore;
        }

        public async Task<OpResult> DepositAsync(decimal amount)
        {
            if (amount <= 0)
                return new OpResult(false, ErrorCode.InvalidAmount, "Invalid deposit amount above 0.", null);

            decimal currentBalance = await _ledgerStore.GetBalanceAsync();
            decimal newBalance = currentBalance + amount;

            ActionRecordBase rec = new DepositAction(amount, _clock.UtcNow);

            await _ledgerStore.SetBalanceAsync(newBalance);
            await _ledgerStore.AddActionAsync(rec);
            await _ledgerStore.SaveChangesAsync();

            string msg = $"Deposited {Formatter.Money(amount)}. Balance: {Formatter.Money(newBalance)}";
            return new OpResult(true, ErrorCode.None, msg, rec);
        }

        public async Task<OpResult> SetPriceAsync(string name, decimal price)
        {
            string cleanName = NameNormalizer.Normalize(name);

            if (cleanName.Length == 0)
                return new OpResult(false, ErrorCode.InvalidName, "Name cannot be empty.", null);

            if (price <= 0)
                return new OpResult(false, ErrorCode.InvalidAmount, "Price must be above 0.", null);

            DateTime utcNow = _clock.UtcNow;
            ActionRecordBase rec = new SetPriceAction(cleanName, price, utcNow);

            await _ledgerStore.SetPriceAsync(cleanName, price, utcNow);
            await _ledgerStore.AddActionAsync(rec);
            await _ledgerStore.SaveChangesAsync();

            string msg = $"Price for {cleanName} set to {Formatter.Money(price)}";
            return new OpResult(true, ErrorCode.None, msg, rec);
        }

        public Task<List<PriceSnapshot>> GetPricesAsync()
        {
            return _ledgerStore.GetPricesAsync();
        }

        public async Task<PriceSnapshot?> GetPriceAsync(string name)
        {
            string cleanName = NameNormalizer.Normalize(name);
            if (string.IsNullOrEmpty(cleanName))
            {
                return null;
            }

            return await _ledgerStore.GetPriceAsync(cleanName);
        }

        public async Task<OpResult> BuyAsync(string name, int qty)
        {
            OpResult result = await _tradeService.BuyAsync(name, qty);

            return result;
        }

        public async Task<OpResult> SellAsync(string name, int qty)
        {
            OpResult result = await _tradeService.SellAsync(name, qty);

            return result;
        }

        // Legacy text report for simple human-readable output.
        // Structured callers should use GetPortfolioValuationAsync().
        public async Task<string> BuildPortfolioReportAsync()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your Portfolio:");

            List<HoldingSnapshot> holdings = await _ledgerStore.GetHoldingsAsync();

            foreach (HoldingSnapshot holding in holdings)
            {
                PriceSnapshot? maybePrice = await _ledgerStore.GetPriceAsync(holding.Name);

                if (maybePrice is not null)
                {
                    decimal value = maybePrice.Price * holding.Quantity;
                    sb.AppendLine($"{holding.Name}: {holding.Quantity} shares, Value: {Formatter.Money(value)}");
                }
                else
                {
                    sb.AppendLine($"{holding.Name}: {holding.Quantity} shares, Value: {Formatter.Money(0m)}");
                }
            }

            decimal balance = await _ledgerStore.GetBalanceAsync();
            sb.AppendLine($"Balance: {Formatter.Money(balance)}");

            return sb.ToString();
        }


        // Text summary version of GetTotalsAsync().
        public async Task<string> BuildTotalsReportAsync()
        {
            TotalsSnapshot totals = await GetTotalsAsync();

            if (totals.ActionCount == 0)
                return "Nothing to report.";

            return $"\nTotals Report:" +
                   $"\nDeposited: {Formatter.Money(totals.TotalDeposited)}" +
                   $"\nSpent on buys: {Formatter.Money(totals.TotalSpentOnBuys)}" +
                   $"\nEarned from sells: {Formatter.Money(totals.TotalEarnedFromSells)}" +
                   $"\nNet cashflow: {Formatter.Money(totals.NetCashflow)}.";
        }

        public async Task<PortfolioValuation> GetPortfolioValuationAsync()
        {
            decimal cashBalance = await _ledgerStore.GetBalanceAsync();

            List<HoldingSnapshot> holdings = await _ledgerStore.GetHoldingsAsync();
            List<PriceSnapshot> prices = await _ledgerStore.GetPricesAsync();

            Dictionary<string, decimal> priceByName = prices.ToDictionary(p => p.Name, p => p.Price);
            List<PortfolioPositionValuation> positions = new();

            foreach (HoldingSnapshot holding in holdings)
            {
                string name = holding.Name;
                int quantity = holding.Quantity;
                bool hasPrice = priceByName.TryGetValue(name, out decimal price);

                // Leave missing prices as null so callers can distinguish
                // unknown valuation from a real zero-valued position.
                if (hasPrice)
                {
                    decimal? currentPrice = price;
                    decimal? positionValue = price * quantity;
                    positions.Add(new PortfolioPositionValuation(name, quantity, currentPrice, positionValue));
                }
                else
                {
                    decimal? currentPrice = null;
                    decimal? positionValue = null;
                    positions.Add(new PortfolioPositionValuation(name, quantity, currentPrice, positionValue));
                }
            }

            List<PortfolioPositionValuation> ordered = positions.OrderBy(p => p.Name).ToList();
            decimal totalPortfolioValue = ordered.Where(p => p.PositionValue is not null).Select(p => p.PositionValue!.Value).Sum();
            decimal totalAccountValue = cashBalance + totalPortfolioValue;
            return new PortfolioValuation(ordered, cashBalance, totalPortfolioValue, totalAccountValue);

        }

        public Task<List<ActionRecordBase>> GetRecentActionsAsync(int take)
        {
            return _ledgerStore.GetRecentActionsAsync(take);
        }

        public Task<List<ActionRecordBase>> GetActionsByTypeAsync(ActionType type, int take)
        {
            return _ledgerStore.GetActionsByTypeAsync(type, take);
        }

        public async Task<TotalsSnapshot> GetTotalsAsync()
        {
            List<ActionRecordBase> actions = await _ledgerStore.GetAllActionsAsync();

            decimal totalDeposited = actions.OfType<DepositAction>().Sum(a => a.Amount);
            decimal totalSpentOnBuys = actions.OfType<BuyAction>().Sum(a => a.Total);
            decimal totalEarnedFromSells = actions.OfType<SellAction>().Sum(a => a.Total);
            decimal netCashflow = totalDeposited - totalSpentOnBuys + totalEarnedFromSells;

            return new TotalsSnapshot(
                ActionCount: actions.Count,
                TotalDeposited: totalDeposited,
                TotalSpentOnBuys: totalSpentOnBuys,
                TotalEarnedFromSells: totalEarnedFromSells,
                NetCashflow: netCashflow
            );
        }

    }
}