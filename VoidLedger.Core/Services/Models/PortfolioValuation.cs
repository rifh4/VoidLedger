namespace VoidLedger.Core.Services.Models
{
    public sealed record PortfolioValuation(IReadOnlyList<PortfolioPositionValuation> Positions, decimal CashBalance, decimal TotalPortfolioValue, decimal TotalAccountValue);
    public sealed record PortfolioPositionValuation(string Name, int Quantity, decimal? CurrentPrice, decimal? PositionValue);
}
