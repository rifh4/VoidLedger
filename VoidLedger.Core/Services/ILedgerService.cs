namespace VoidLedger.Core;

public interface ILedgerService
{
    OpResult SetPrice(string name, decimal price);
    OpResult Deposit(decimal amount);
     string BuildPortfolioReport();
    string BuildRecentActionsReport(int n);
    string BuildTotalsReport();
    //string RunSmokeTests();
    string BuildActionsByTypeReport(ActionType type, int n);
    Task<OpResult> DepositAsync(decimal amount);
    Task<OpResult> SetPriceAsync(string name, decimal price);
    Task<OpResult> BuyAsync(string name, int qty);
    Task<OpResult> SellAsync(string name, int qty);
}