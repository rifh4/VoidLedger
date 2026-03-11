namespace VoidLedger.Core;

public record TradeResult(
    bool Ok,
    ErrorCode Code,
    string Message,
    string? Name,
    decimal? UnitPrice,
    decimal? Total,
    decimal? NewBalance,
    int? NewHoldingQuantity
);