using VoidLedger.Core.Services.Models;
using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServicePortfolioValuationTests
    {
        [Fact]
        public async Task GetPortfolioValuation_WhenMissingPrice_ShouldReturnNullPriceAndIgnoreInTotals()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.Store.SetBalanceAsync(100m);

            await system.Store.SetHoldingQuantityAsync("AAA", 2);
            await system.Store.SetHoldingQuantityAsync("NOPRICE", 1);

            await system.Store.SetPriceAsync("AAA", 10m, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            PortfolioValuation valuation = await system.LedgerService.GetPortfolioValuationAsync();

            Assert.Equal(2, valuation.Positions.Count);

            PortfolioPositionValuation first = valuation.Positions[0];
            PortfolioPositionValuation second = valuation.Positions[1];

            Assert.Equal("AAA", first.Name);
            Assert.Equal("NOPRICE", second.Name);

            Assert.Equal(2, first.Quantity);
            Assert.Equal(10m, first.CurrentPrice);
            Assert.Equal(20m, first.PositionValue);

            Assert.Equal(1, second.Quantity);
            Assert.Null(second.CurrentPrice);
            Assert.Null(second.PositionValue);

            Assert.Equal(100m, valuation.CashBalance);
            Assert.Equal(20m, valuation.TotalPortfolioValue);
            Assert.Equal(120m, valuation.TotalAccountValue);
        }
    }
}