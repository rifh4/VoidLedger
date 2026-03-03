namespace VoidLedger.Core;

public sealed class TradeService
{
    // Encapsulates buy/sell rules so the ledger service can focus on orchestration + logging.
    private readonly Account _account;
    private readonly PriceBook _prices;
    private readonly Portfolio _portfolio;

    public TradeService(Account account, PriceBook prices, Portfolio portfolio)
    {
        _account = account;
        _prices = prices;
        _portfolio = portfolio;
    }

    public TradeResult Buy(string name, int qty)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new TradeResult(false, ErrorCode.InvalidName, "Name cannot be empty");
        
        if (qty <= 0)
            return new TradeResult(false, ErrorCode.InvalidAmount, "Quantity must be above 0");

        if (!_prices.TryGetPrice(name, out decimal price))
            return new TradeResult(false, ErrorCode.MissingPrice, "Price not set");

        decimal cost = price * qty;

        if (!_account.Withdraw(cost))
            return new TradeResult(false, ErrorCode.InsufficientFunds, "Insufficient funds");

        _portfolio.Add(name, qty);

        string msg = $"Bought {qty} {name} for {Formatter.Money(cost)}. Balance: {Formatter.Money(_account.Balance)}";
        return new TradeResult(true, ErrorCode.None, msg);
    }

    public TradeResult Sell(string name, int qty)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new TradeResult(false, ErrorCode.InvalidName, "Name cannot be empty");

        if (qty <= 0)
            return new TradeResult(false, ErrorCode.InvalidAmount, "Quantity must be > 0");

        if (!_prices.TryGetPrice(name, out decimal price))
            return new TradeResult(false, ErrorCode.MissingPrice, "Price not set");

        // Distinguish MissingHolding vs Oversell without refactoring rules:
        if (!_portfolio.TryGetQty(name, out int ownedQty))
            return new TradeResult(false, ErrorCode.MissingHolding, "No holdings to sell");

        if (qty > ownedQty)
            return new TradeResult(false, ErrorCode.Oversell, "Not enough holdings to sell");

        // Now we know the remove will succeed.
        _portfolio.Remove(name, qty);

        decimal revenue = price * qty;
        _account.Deposit(revenue);

        string msg = $"Sold {qty} {name} for {Formatter.Money(revenue)}. Balance: {Formatter.Money(_account.Balance)}";
        return new TradeResult(true, ErrorCode.None, msg);
    }
}