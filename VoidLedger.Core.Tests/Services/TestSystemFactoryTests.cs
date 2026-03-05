using VoidLedger.Core.Tests.Support;
namespace VoidLedger.Core.Tests.Services;

public sealed class TestSystemFactoryTests
{
    [Fact]
    public void Create_ShouldStartWithEmptyState()
    {
        TestSystem system = TestSystemFactory.Create();
        Assert.Equal(0m, system.Balance);
        Assert.Equal(0, system.ActionCount);
        Assert.Equal(0, system.GetHoldingQty("WATER"));
        Assert.Equal(0m, system.GetPrice("WATER"));
    }
}