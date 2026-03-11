namespace VoidLedger.Api.Contracts
{
    public sealed record RecentActionsResponse(int Count, IReadOnlyList<ActionItemResponse> Items );
}
