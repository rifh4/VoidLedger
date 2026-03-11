namespace VoidLedger.Core;

public sealed record SetPriceAction(string Name, decimal Price, DateTime At)
    : ActionRecordBase(ActionType.SetPrice, At)
{
    public override string Describe()
        => $"SET_PRICE | {Name} | Price={Formatter.Money(Price)} | At={Formatter.UtcStamp(At)}";
}