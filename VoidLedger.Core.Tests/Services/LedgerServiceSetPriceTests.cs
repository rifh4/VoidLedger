using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceSetPriceTests
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
    }
}