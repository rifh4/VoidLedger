namespace VoidLedger.Core;

public sealed class PriceBook
{
    // In-memory pricing table; validation happens on set.
    private readonly Dictionary<string, decimal> _pricing;

    public PriceBook(Dictionary<string, decimal> pricing)
    {
        _pricing = pricing;
    }

    public bool SetPrice(string name, decimal price, out string msg)
    {
        string clean = (name ?? "").Trim();
        if (clean.Length == 0)
        {
            msg = "Please enter a valid name";
            return false;
        }

        if (price <= 0)
        {
            msg = "Please enter a number above 0";
            return false;
        }

        _pricing[clean] = price;
        msg = $"Set the price of {clean} to {Formatter.Money(price)}";
        return true;
    }

    public bool TryGetPrice(string name, out decimal price) =>
        _pricing.TryGetValue(name, out price);
}