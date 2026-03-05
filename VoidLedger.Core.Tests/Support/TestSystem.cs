namespace VoidLedger.Core.Tests.Support
{
    internal sealed class TestSystem
    {
        internal LedgerService LedgerService { get; }
        internal Account Account { get; }
        internal Portfolio Portfolio { get; }
        internal PriceBook PriceBook { get; }
        internal List<ActionRecordBase> ActionRecords { get; }
        internal decimal Balance => Account.Balance;
        internal int ActionCount => ActionRecords.Count;

        internal TestSystem(LedgerService ledgerService, 
            Account account, 
            Portfolio portfolio, 
            PriceBook priceBook, 
            List<ActionRecordBase> actionRecords)
        {
            LedgerService = ledgerService;
            Account = account;
            Portfolio = portfolio;
            PriceBook = priceBook;
            ActionRecords = actionRecords;

        }

        internal int GetHoldingQty(string name)
        {
            if (Portfolio.TryGetQty(name, out int qty))
            {
                return qty;
            }
            return 0;
        }

        internal decimal GetPrice(string name)
        {
            if (PriceBook.TryGetPrice(name, out decimal price))
            {
                return price;
            }
            return 0;
        }
    }
}
