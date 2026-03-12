using VoidLedger.Core.Services.Models;
using VoidLedger.Core.Stores;

namespace VoidLedger.Core;

public interface ILedgerService
{
    Task<OpResult> DepositAsync(decimal amount);
    Task<OpResult> SetPriceAsync(string name, decimal price);
    Task<OpResult> BuyAsync(string name, int qty);
    Task<OpResult> SellAsync(string name, int qty);
    Task<string> BuildPortfolioReportAsync();
    Task<string> BuildRecentActionsReportAsync(int n);
    Task<string> BuildActionsByTypeReportAsync(ActionType type, int n);
    Task<string> BuildTotalsReportAsync();
    Task<List<PriceSnapshot>> GetPricesAsync();
    Task<PortfolioValuation> GetPortfolioValuationAsync();
    Task<List<ActionRecordBase>> GetRecentActionsAsync(int take);
    Task<List<ActionRecordBase>> GetActionsByTypeAsync(ActionType type, int take);
    Task<TotalsSnapshot> GetTotalsAsync();
    Task<PriceSnapshot?> GetPriceAsync(string name);
}