namespace VoidLedger.Core.Stores
{
    public sealed record PriceSnapshot(string Name, decimal Price, decimal? PreviousPrice, DateTime UpdatedAtUtc);
}
