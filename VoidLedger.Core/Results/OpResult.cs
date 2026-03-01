namespace VoidLedger.Core;

// Unified “service result” object for success/failure + optional log record.
public record OpResult(
    bool Ok,
    ErrorCode Code,
    string Message,
    ActionRecordBase? Record = null
);
