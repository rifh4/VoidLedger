namespace VoidLedger.Api.Contracts
{
    public sealed record PortfolioValuationResponse(
        IReadOnlyList<PortfolioPositionResponse> Positions, 
        decimal CashBalance, 
        decimal TotalPortfolioValue, 
        decimal TotalAccountValue
        );
}
