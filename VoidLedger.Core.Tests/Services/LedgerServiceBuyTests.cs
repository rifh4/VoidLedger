using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceBuyTests
    {
        [Fact]
        public async Task Buy_WhenPriceIsMissing_ShouldNotLogAndNotBuy()
        {
            TestSystem system = TestSystemFactory.Create();

            OpResult result = await system.LedgerService.BuyAsync("WATER", 1);

            Assert.False(result.Ok);
            Assert.Equal(ErrorCode.MissingPrice, result.Code);
            Assert.Null(result.Record);
            Assert.Equal(0, system.ActionCount);
            Assert.Equal(0m, system.Balance);
            Assert.Equal(0, system.GetHoldingQty("WATER"));
        }

        [Fact]
        public async Task Buy_WhenInsufficientFunds_ShouldNotLogAndNotBuy()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.SetPriceAsync("WATER", 10m);
            await system.LedgerService.DepositAsync(5m);
            int before = system.ActionCount;

            OpResult result = await system.LedgerService.BuyAsync("WATER", 1);

            Assert.False(result.Ok);
            Assert.Equal(ErrorCode.InsufficientFunds, result.Code);
            Assert.Null(result.Record);
            Assert.Equal(0, system.GetHoldingQty("WATER"));
            Assert.Equal(5m, system.Balance);
            Assert.Equal(before, system.ActionCount);
        }

        [Fact]
        public async Task Buy_WhenValid_ShouldLogAndBuy()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.SetPriceAsync("WATER", 10m);
            await system.LedgerService.DepositAsync(100m);
            int before = system.ActionCount;

            OpResult result = await system.LedgerService.BuyAsync("WATER", 3);

            Assert.True(result.Ok);
            Assert.Equal(ErrorCode.None, result.Code);
            Assert.NotNull(result.Record);
            Assert.Equal(70m, system.Balance);
            Assert.Equal(3, system.GetHoldingQty("WATER"));
            Assert.Equal(before + 1, system.ActionCount);
        }
    }
}