using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceValidationTests
    {
        [Fact]
        public async Task Buy_WhenNameInvalid_ShouldNotLogAndNotBuy()
        {
            TestSystem system = TestSystemFactory.Create();

            OpResult result = await system.LedgerService.BuyAsync("", 1);

            Assert.False(result.Ok);
            Assert.Equal(ErrorCode.InvalidName, result.Code);
            Assert.Null(result.Record);
            Assert.Equal(0, system.ActionCount);
            Assert.Equal(0m, system.Balance);
            Assert.Equal(0, system.GetHoldingQty("WATER"));
        }
    }
}