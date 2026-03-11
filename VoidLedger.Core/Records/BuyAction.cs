namespace VoidLedger.Core;

public sealed record BuyAction(string Name, int Qty, decimal UnitPrice, decimal Total, DateTime At)
    : ActionRecordBase(ActionType.Buy, At)
{
    public override string Describe()
        => $"BUY | {Name} x{Qty} | Unit={Formatter.Money(UnitPrice)} | Total={Formatter.Money(Total)} | At={Formatter.UtcStamp(At)}";
}