using System.Text.Json.Serialization;
using VoidLedger.Core;

namespace VoidLedger.Api.Contracts
{
    public sealed record ActionItemResponse(
        [property: JsonConverter(typeof(JsonStringEnumConverter))] ActionType Type,
        DateTime At,
        string? Name,
        int? Quantity,
        decimal? Amount,
        decimal? UnitPrice,
        decimal? Total,
        decimal? Price
    );
}
