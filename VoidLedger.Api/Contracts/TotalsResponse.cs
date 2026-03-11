namespace VoidLedger.Api.Contracts
{
    public sealed record TotalsResponse(
        int ActionCount,
        decimal TotalDeposited,
        decimal TotalSpentOnBuys,
        decimal TotalEarnedFromSells,
        decimal NetCashflow
    );
}
