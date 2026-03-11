using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceRecentActionsJsonTests
    {
        [Fact]
        public async Task GetRecentActions_WhenTakeIsSmallerThanHistory_ShouldReturnLastTakeInOrder()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.DepositAsync(100m);         
            await system.LedgerService.SetPriceAsync("AAA", 10m);  
            await system.LedgerService.BuyAsync("AAA", 1);         
            await system.LedgerService.SetPriceAsync("AAA", 11m);  

            List<ActionRecordBase> recent = await system.LedgerService.GetRecentActionsAsync(2);

            Assert.Equal(2, recent.Count);
            Assert.Equal(ActionType.Buy, recent[0].Type);
            Assert.Equal(ActionType.SetPrice, recent[1].Type);
        }

        [Fact]
        public async Task GetRecentActions_WhenTakeIsLargerThanHistory_ShouldReturnAll()
        {
            TestSystem system = TestSystemFactory.Create();

            await system.LedgerService.DepositAsync(100m);
            await system.LedgerService.SetPriceAsync("AAA", 10m);

            List<ActionRecordBase> recent = await system.LedgerService.GetRecentActionsAsync(10);

            Assert.Equal(2, recent.Count);
        }
    }
}
