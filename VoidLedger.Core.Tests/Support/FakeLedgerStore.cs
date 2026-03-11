namespace VoidLedger.Core.Tests.Support
{
    internal sealed class FakeLedgerStore : ILedgerStore
    {
        private decimal _balance;
        private readonly Dictionary<string, decimal> _prices = new();
        private readonly Dictionary<string, int> _holdings = new();
        private readonly List<ActionRecordBase> _actions = new();
        internal decimal BalanceSnapshot => _balance;
        internal IReadOnlyList<ActionRecordBase> ActionsSnapshot => _actions;

        public FakeLedgerStore(decimal startingBalance)
        {
            _balance = startingBalance;
        }

        public Task<decimal> GetBalanceAsync()
        {
            return Task.FromResult(_balance);
        }

        public Task SetBalanceAsync(decimal newBalance)
        {
            _balance = newBalance;
            return Task.CompletedTask;
        }

        public Task<decimal?> GetPriceAsync(string name)
        {
            bool found = _prices.TryGetValue(name, out decimal price);
            return Task.FromResult(found ? (decimal?)price : null);
        }

        public Task SetPriceAsync(string name, decimal price)
        {
            _prices[name] = price;
            return Task.CompletedTask;
        }

        public Task<int?> GetHoldingQuantityAsync(string name)
        {
            bool found = _holdings.TryGetValue(name, out int qty);
            return Task.FromResult(found ? (int?)qty : null);
        }

        public Task SetHoldingQuantityAsync(string name, int quantity)
        {
            if (quantity == 0)
            {
                _holdings.Remove(name);
                return Task.CompletedTask;
            }

            _holdings[name] = quantity;
            return Task.CompletedTask;
        }

        public Task<List<HoldingSnapshot>> GetHoldingsAsync()
        {
            List<HoldingSnapshot> holdings = _holdings
                .Select(kvp => new HoldingSnapshot(kvp.Key, kvp.Value))
                .ToList();

            return Task.FromResult(holdings);
        }

        public Task AddActionAsync(ActionRecordBase action)
        {
            _actions.Add(action);
            return Task.CompletedTask;
        }

        public Task<List<ActionRecordBase>> GetRecentActionsAsync(int take)
        {
            List<ActionRecordBase> actions = _actions
                .TakeLast(take)
                .ToList();

            return Task.FromResult(actions);
        }

        public Task<List<ActionRecordBase>> GetActionsByTypeAsync(ActionType type, int take)
        {
            List<ActionRecordBase> actions = _actions
                .Where(a => a.Type == type)
                .TakeLast(take)
                .ToList();

            return Task.FromResult(actions);
        }

        public Task<List<ActionRecordBase>> GetAllActionsAsync()
        {
            return Task.FromResult(_actions.ToList());
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        internal int GetHoldingQtySnapshot(string name)
        {
            return _holdings.TryGetValue(name, out int qty) ? qty : 0;
        }

        internal decimal GetPriceSnapshot(string name)
        {
            return _prices.TryGetValue(name, out decimal price) ? price : 0m;
        }
    }
}