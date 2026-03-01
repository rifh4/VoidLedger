
namespace VoidLedger.Core;

public record TradeResult(
    bool Ok,
    ErrorCode Code,
    string Message
);
