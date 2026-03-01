namespace VoidLedger.Core;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}