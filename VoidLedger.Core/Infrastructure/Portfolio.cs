namespace VoidLedger.Core;

public sealed class Portfolio
{
    // Single source of truth for holdings; no copies.
    private readonly Dictionary<string, int> _holdings;

    public Portfolio(Dictionary<string, int> holdings)
    {
        _holdings = holdings;
    }

    public void Add(string name, int qty)
    {
        if (qty <= 0) return;
        _holdings.TryGetValue(name, out int ownedQty);
        _holdings[name] = ownedQty + qty;
    }

    public bool Remove(string name, int qty)
    {
        if (qty <= 0) return false;
        if (!_holdings.TryGetValue(name, out int ownedQty)) return false;
        if (qty > ownedQty) return false;

        int newQty = ownedQty - qty;
        if (newQty == 0) _holdings.Remove(name);
        else _holdings[name] = newQty;

        return true;
    }

    // Minimal helper (no behavior change) to support MissingHolding vs Oversell.
    public bool TryGetQty(string name, out int qty) =>
        _holdings.TryGetValue(name, out qty);

    public IEnumerable<KeyValuePair<string, int>> GetHoldings() => _holdings;
}