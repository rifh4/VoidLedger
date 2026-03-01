namespace VoidLedger.Core;

public sealed class FixedClock : IClock
{
    public DateTime UtcNow { get; }

    public FixedClock(DateTime utcNow)
    {
        if (utcNow.Kind != DateTimeKind.Utc)
            utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);

        UtcNow = utcNow;
    }
}