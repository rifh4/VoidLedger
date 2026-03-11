namespace VoidLedger.Api.Contracts
{
    public sealed record PortfolioPositionResponse(string Name, int Quantity, decimal? CurrentPrice, decimal? PositionValue);
}
