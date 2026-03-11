using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceReportsTests
    {
        [Fact]
        public async Task TestReports()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.SetPriceAsync("WATER", 10m);
            await system.LedgerService.DepositAsync(100m);
            await system.LedgerService.BuyAsync("WATER", 3);
            await system.LedgerService.SellAsync("WATER", 1);

            string report = await system.LedgerService.BuildTotalsReportAsync();

            Assert.Equal(80m, system.Balance);
            Assert.Contains("Totals Report:", report);
            Assert.Contains("Deposited: 100.00", report);
            Assert.Contains("Earned from sells: 10.00", report);
            Assert.Contains("Spent on buys: 30.00", report);
            Assert.Contains("Net cashflow: 80.00", report);
        }
    }
}