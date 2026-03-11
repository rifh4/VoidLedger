using VoidLedger.Core.Services;

namespace VoidLedger.Core.Tests.Support
{
    internal static class TestSystemFactory
    {
        internal static TestSystem Create()
        {
            DateTime fixedUtc = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            FixedClock clock = new FixedClock(fixedUtc);
            FakeLedgerStore store = new FakeLedgerStore(0m);
            ITradeService tradeService = new TradeService(store, clock);
            LedgerService ledger = new LedgerService(tradeService, clock, store);

            return new TestSystem(ledger, store);
        }
    }
}