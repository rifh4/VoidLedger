namespace VoidLedger.Core;

public sealed record DepositAction(decimal Amount, DateTime At)
    : ActionRecordBase(ActionType.Deposit, At)
{
    public override string Describe()
        => $"DEPOSIT | Amount={Formatter.Money(Amount)} | At={Formatter.UtcStamp(At)}";
}