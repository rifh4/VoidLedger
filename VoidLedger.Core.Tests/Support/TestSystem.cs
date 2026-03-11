namespace VoidLedger.Core.Tests.Support
{
    internal sealed class TestSystem
    {
        internal LedgerService LedgerService { get; }
        internal FakeLedgerStore Store { get; }

        internal decimal Balance => Store.BalanceSnapshot;
        internal int ActionCount => Store.ActionsSnapshot.Count;
        internal IReadOnlyList<ActionRecordBase> ActionRecords => Store.ActionsSnapshot;

        internal TestSystem(LedgerService ledgerService, FakeLedgerStore store)
        {
            LedgerService = ledgerService;
            Store = store;
        }

        internal int GetHoldingQty(string name)
        {
            return Store.GetHoldingQtySnapshot(name);
        }

        internal decimal GetPrice(string name)
        {
            return Store.GetPriceSnapshot(name);
        }
    }
}