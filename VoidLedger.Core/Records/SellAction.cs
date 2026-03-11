namespace VoidLedger.Core;

public sealed record SellAction(string Name, int Qty, decimal UnitPrice, decimal Total, DateTime At)
    : ActionRecordBase(ActionType.Sell, At)
{
    public override string Describe()
        => $"SELL | {Name} x{Qty} | Unit={Formatter.Money(UnitPrice)} | Total={Formatter.Money(Total)} | At={Formatter.UtcStamp(At)}";
}