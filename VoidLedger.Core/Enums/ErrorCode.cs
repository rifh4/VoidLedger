namespace VoidLedger.Core;

public enum ErrorCode
{
    None,
    InvalidAmount,
    MissingPrice,
    MissingHolding,
    Oversell,
    InsufficientFunds,
    InvalidName,
    Unknown
}
