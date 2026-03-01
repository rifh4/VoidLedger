namespace VoidLedger.Core;

public sealed record SellAction(string Symbol, int Qty, decimal UnitPrice, decimal Total, DateTime At)
    : ActionRecordBase(ActionType.Sell, At)
{
    public override string Describe()
        => $"SELL | {Symbol} x{Qty} | Unit={Formatter.Money(UnitPrice)} | Total={Formatter.Money(Total)} | At={Formatter.UtcStamp(At)}";
}