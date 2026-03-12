using VoidLedger.Core.Stores;
using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServicePriceTests
    {
        [Fact]
        public async Task SetPrice_WhenValid_ShouldLogAndSetPrice()
        {
            TestSystem system = TestSystemFactory.Create();

            OpResult result = await system.LedgerService.SetPriceAsync("WATER", 10m);

            Assert.True(result.Ok);
            Assert.Equal(ErrorCode.None, result.Code);
            Assert.NotNull(result.Record);
            Assert.Equal(1, system.ActionCount);
            Assert.Equal(10m, system.GetPrice("WATER"));
        }

        [Fact]
        public async Task SetPrice_WhenInvalidAmount_ShouldNotLogAndNotSetPrice()
        {
            TestSystem system = TestSystemFactory.Create();

            OpResult result = await system.LedgerService.SetPriceAsync("WATER", -1m);

            Assert.False(result.Ok);
            Assert.Equal(ErrorCode.InvalidAmount, result.Code);
            Assert.Null(result.Record);
            Assert.Equal(0, system.ActionCount);
            Assert.Equal(0m, system.GetPrice("WATER"));
        }

        [Fact]
        public async Task GetPrices_WhenMultiplePrices_ShouldReturnOrderedByNameAndNormalized()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.SetPriceAsync("abc", 1m);
            await system.LedgerService.SetPriceAsync(" AAA ", 2m);
            await system.LedgerService.SetPriceAsync("water", 3m);

            List<PriceSnapshot> prices = await system.LedgerService.GetPricesAsync();

            Assert.Equal(3, prices.Count);
            Assert.Equal("AAA", prices[0].Name);
            Assert.Equal("ABC", prices[1].Name);
            Assert.Equal("WATER", prices[2].Name);
            Assert.Equal(2m, prices[0].Price);
            Assert.Equal(1m, prices[1].Price);
            Assert.Equal(3m, prices[2].Price);
        }

        [Fact]
        public async Task GetPrice_WhenFound_ShouldReturnSnapshot()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.SetPriceAsync("abc", 1m);
            PriceSnapshot? priceSnapshot = await system.LedgerService.GetPriceAsync("abc");
            Assert.NotNull(priceSnapshot);
            Assert.Equal("ABC", priceSnapshot.Name);
            Assert.Equal(1m, priceSnapshot.Price);
        }

        [Fact]
        public async Task GetPrice_WhenMissing_ShouldReturnNull()
        {
            TestSystem system = TestSystemFactory.Create();

            PriceSnapshot? priceSnapshot = await system.LedgerService.GetPriceAsync("aaa");
            
            Assert.Null(priceSnapshot);
        }

        [Fact]
        public async Task GetPrice_WhenInputNeedsNormalization_ShouldReturnNormalizedSnapshot()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.SetPriceAsync("AAA", 1m);

            PriceSnapshot? priceSnapshot = await system.LedgerService.GetPriceAsync(" aaa ");

            Assert.NotNull(priceSnapshot);
            Assert.Equal("AAA", priceSnapshot.Name);
            Assert.Equal(1m, priceSnapshot.Price);

        }
    }
}