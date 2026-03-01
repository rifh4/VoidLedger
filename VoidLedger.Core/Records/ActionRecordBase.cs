namespace VoidLedger.Core;

public abstract record ActionRecordBase(ActionType Type, DateTime At)
{
    public abstract string Describe();
}