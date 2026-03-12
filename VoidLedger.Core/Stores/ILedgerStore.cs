using VoidLedger.Core.Stores;

namespace VoidLedger.Core;

public interface ILedgerStore
{
    Task<decimal> GetBalanceAsync();
    Task SetBalanceAsync(decimal newBalance);

    Task<PriceSnapshot?> GetPriceAsync(string name);
    Task SetPriceAsync(string name, decimal price, DateTime updatedAtUtc);

    Task<int?> GetHoldingQuantityAsync(string name);
    Task SetHoldingQuantityAsync(string name, int quantity);
    Task<List<HoldingSnapshot>> GetHoldingsAsync();

    Task AddActionAsync(ActionRecordBase action);
    Task<List<ActionRecordBase>> GetRecentActionsAsync(int take);
    Task<List<ActionRecordBase>> GetActionsByTypeAsync(ActionType type, int take);
    Task<List<ActionRecordBase>> GetAllActionsAsync();
    Task SaveChangesAsync();
    Task<List<PriceSnapshot>> GetPricesAsync();
}