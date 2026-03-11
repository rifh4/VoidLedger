using VoidLedger.Core.Tests.Support;

namespace VoidLedger.Core.Tests.Services
{
    public sealed class LedgerServiceDepositTests
    {
        [Fact]
        public async Task Deposit_WhenValid_ShouldLogAndDeposit()
        {
            TestSystem system = TestSystemFactory.Create();

            OpResult result = await system.LedgerService.DepositAsync(100m);

            Assert.True(result.Ok);
            Assert.Equal(ErrorCode.None, result.Code);
            Assert.NotNull(result.Record);
            Assert.Equal(100m, system.Balance);
            Assert.Equal(1, system.ActionCount);
        }

        [Fact]
        public async Task Deposit_WhenInvalid_ShouldNotLogAndNotDeposit()
        {
            TestSystem system = TestSystemFactory.Create();

            OpResult result = await system.LedgerService.DepositAsync(-1m);

            Assert.False(result.Ok);
            Assert.Equal(ErrorCode.InvalidAmount, result.Code);
            Assert.Null(result.Record);
            Assert.Equal(0m, system.Balance);
            Assert.Equal(0, system.ActionCount);
        }
    }
}