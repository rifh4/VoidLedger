namespace VoidLedger.Core;

public interface IClock
{
    DateTime UtcNow { get; }
}