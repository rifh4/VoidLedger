using VoidLedger.Core.Services.Models;
using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceTotalsJsonTests
    {
        [Fact]
        public async Task GetTotals_WhenActionsExist_ShouldReturnCorrectTotals()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.DepositAsync(200m);

            await system.LedgerService.SetPriceAsync("AAA", 10m);
            await system.LedgerService.BuyAsync("AAA", 3);   

            await system.LedgerService.SetPriceAsync("AAA", 12m);
            await system.LedgerService.SellAsync("AAA", 2);  

            TotalsSnapshot totals = await system.LedgerService.GetTotalsAsync();

            Assert.Equal(system.ActionCount, totals.ActionCount);
            Assert.Equal(200m, totals.TotalDeposited);
            Assert.Equal(30m, totals.TotalSpentOnBuys);
            Assert.Equal(24m, totals.TotalEarnedFromSells);
            Assert.Equal(194m, totals.NetCashflow);
        }

        [Fact]
        public async Task GetTotals_WhenNoActions_ShouldReturnZeros()
        {
            TestSystem system = TestSystemFactory.Create();

            TotalsSnapshot totals = await system.LedgerService.GetTotalsAsync();

            Assert.Equal(0, totals.ActionCount);
            Assert.Equal(0m, totals.TotalDeposited);
            Assert.Equal(0m, totals.TotalSpentOnBuys);
            Assert.Equal(0m, totals.TotalEarnedFromSells);
            Assert.Equal(0m, totals.NetCashflow);
        }
    }
}
