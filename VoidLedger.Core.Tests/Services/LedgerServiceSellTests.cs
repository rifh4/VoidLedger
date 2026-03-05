using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceSellTests
    {
        [Fact]
        public void Sell_WhenHoldingsIsMissing_ShouldNotLogAndNotSell()
        {
            TestSystem system = TestSystemFactory.Create();
            system.LedgerService.SetPrice("WATER", 10m);
            int before = system.ActionCount;

            OpResult result = system.LedgerService.Sell("WATER", 1);

            Assert.False(result.Ok);
            Assert.Equal(ErrorCode.MissingHolding, result.Code);
            Assert.Null(result.Record);
            Assert.Equal(before, system.ActionCount);  
            Assert.Equal(0m, system.Balance);
            Assert.Equal(0, system.GetHoldingQty("WATER"));
        }
        [Fact]
        public void Sell_WhenOverSell_ShouldNotLogAndNotSell()
        {
            TestSystem system = TestSystemFactory.Create();
            system.LedgerService.SetPrice("WATER", 10m);
            system.LedgerService.Deposit(100m);
            system.LedgerService.Buy("WATER", 2);
            int before = system.ActionCount;

            Assert.Equal(2, system.GetHoldingQty("WATER"));

            OpResult result = system.LedgerService.Sell("WATER", 3); 
            
            Assert.False(result.Ok);
            Assert.Equal(ErrorCode.Oversell, result.Code);
            Assert.Null(result.Record);
            Assert.Equal(80m, system.Balance);
            Assert.Equal(2, system.GetHoldingQty("WATER"));
            Assert.Equal(before, system.ActionCount);
        }
        [Fact]
        public void Sell_WhenValid_ShouldLogAndSell()
        {
            TestSystem system = TestSystemFactory.Create();
            system.LedgerService.SetPrice("WATER", 10m);
            system.LedgerService.Deposit(100m);
            system.LedgerService.Buy("WATER", 3);
            int before = system.ActionCount;

            OpResult result = system.LedgerService.Sell("WATER", 2);

            Assert.True(result.Ok);
            Assert.Equal(ErrorCode.None, result.Code);
            Assert.NotNull(result.Record);
            Assert.Equal(90m, system.Balance);
            Assert.Equal(1, system.GetHoldingQty("WATER"));
            Assert.Equal(before + 1, system.ActionCount);
        }
        [Fact]
        public void Sell_WhenZero_RemoveHoldings()
        {
            TestSystem system = TestSystemFactory.Create();
            system.LedgerService.SetPrice("WATER", 10m);
            system.LedgerService.Deposit(100m);
            system.LedgerService.Buy("WATER", 2);
            int before = system.ActionCount;

            OpResult result = system.LedgerService.Sell("WATER", 2);

            Assert.True(result.Ok);
            Assert.Equal(0, system.GetHoldingQty("WATER"));
            Assert.Equal(100m, system.Balance);
            Assert.Equal(before + 1, system.ActionCount);
        }
    }
}
