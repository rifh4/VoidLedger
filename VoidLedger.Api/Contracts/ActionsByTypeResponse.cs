using System.Text.Json.Serialization;
using VoidLedger.Core;

namespace VoidLedger.Api.Contracts
{
    public sealed record ActionsByTypeResponse(
        [property: JsonConverter(typeof(JsonStringEnumConverter))] ActionType Type,
        int Count,
        IReadOnlyList<ActionItemResponse> Items
    );
}
