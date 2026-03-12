namespace VoidLedger.Api.Contracts
{
    public sealed record PriceResponse(string Name, decimal Price, decimal? PreviousPrice, DateTime UpdatedAtUtc, decimal? ChangeAmount, string Direction);
}
