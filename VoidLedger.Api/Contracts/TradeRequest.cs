using System.ComponentModel.DataAnnotations;

namespace VoidLedger.Api.Contracts;

public sealed record TradeRequest(
    [Required, MinLength(1)] string Name,
    int Qty
);