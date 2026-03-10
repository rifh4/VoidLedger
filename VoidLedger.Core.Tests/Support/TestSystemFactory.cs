namespace VoidLedger.Core.Tests.Support
{
    internal static class TestSystemFactory
    {
        internal static TestSystem Create()
        {
            DateTime fixedUtc = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            FixedClock clock = new FixedClock(fixedUtc);
            List<ActionRecordBase> log = new List<ActionRecordBase>();
            Account account = new Account(0m);
            PriceBook priceBook = new PriceBook(new Dictionary<string, decimal>());
            Portfolio portfolio = new Portfolio(new Dictionary<string, int>());
            TradeService tradeService = new TradeService(account, priceBook, portfolio );
            FakeAccountStore store = new FakeAccountStore(0m);
            LedgerService ledger = new LedgerService(account ,priceBook , portfolio, tradeService , log, clock, store);

            return new TestSystem(ledger, account, portfolio, priceBook, log);
        }
    }
}
