namespace VoidLedger.Core.Services.Models
{
    public sealed record TotalsSnapshot(
        int ActionCount,
        decimal TotalDeposited,
        decimal TotalSpentOnBuys,
        decimal TotalEarnedFromSells,
        decimal NetCashflow
    );
}
