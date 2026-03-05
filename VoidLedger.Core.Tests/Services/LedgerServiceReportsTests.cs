using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceReportsTests
    {
        [Fact]
        public void TestReports()
        {
            TestSystem system = TestSystemFactory.Create();
            system.LedgerService.SetPrice("WATER", 10m);
            system.LedgerService.Deposit(100m);
            system.LedgerService.Buy("WATER", 3); 
            system.LedgerService.Sell("WATER", 1);
            
            string report = system.LedgerService.BuildTotalsReport();

            Assert.Equal(80m, system.Balance);
            Assert.Contains("Totals Report:", report);
            Assert.Contains("Deposited: 100.00", report);
            Assert.Contains("Earned from sells: 10.00", report);
            Assert.Contains("Spent on buys: 30.00", report);
            Assert.Contains("Net cashflow: 80.00", report);

        }
    }
}
