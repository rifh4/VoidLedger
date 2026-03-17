namespace VoidLedger.Core;

// Unified service result for success/failure plus an optional action log.
public record OpResult(
    bool Ok,
    ErrorCode Code,
    string Message,
    ActionRecordBase? Record = null
);
