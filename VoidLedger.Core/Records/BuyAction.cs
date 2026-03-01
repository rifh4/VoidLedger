namespace VoidLedger.Core;

public sealed record BuyAction(string Symbol, int Qty, decimal UnitPrice, decimal Total, DateTime At)
    : ActionRecordBase(ActionType.Buy, At)
{
    public override string Describe()
        => $"BUY | {Symbol} x{Qty} | Unit={Formatter.Money(UnitPrice)} | Total={Formatter.Money(Total)} | At={Formatter.UtcStamp(At)}";
}