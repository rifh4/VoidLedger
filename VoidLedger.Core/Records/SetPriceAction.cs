namespace VoidLedger.Core;

public sealed record SetPriceAction(string Symbol, decimal Price, DateTime At)
    : ActionRecordBase(ActionType.SetPrice, At)
{
    public override string Describe()
        => $"SET_PRICE | {Symbol} | Price={Formatter.Money(Price)} | At={Formatter.UtcStamp(At)}";
}