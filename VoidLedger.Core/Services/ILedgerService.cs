namespace VoidLedger.Core;

public interface ILedgerService
{
    OpResult SetPrice(string name, decimal price);
    OpResult Deposit(decimal amount);
    OpResult Buy(string name, int qty);
    OpResult Sell(string name, int qty);

    string BuildPortfolioReport();
    string BuildRecentActionsReport(int n);
    string BuildTotalsReport();
    string RunSmokeTests();
    string BuildActionsByTypeReport(ActionType type, int n);
}