namespace VoidLedger.Core.Tests.Support
{
    internal sealed class FakeAccountStore : ILedgerStore
    {
        private decimal _balance;

        public FakeAccountStore(decimal startingBalance)
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
            throw new NotImplementedException();
        }

        public Task SetPriceAsync(string name, decimal price)
        {
            throw new NotImplementedException();
        }

        public Task<int?> GetHoldingQuantityAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task SetHoldingQuantityAsync(string name, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<List<HoldingSnapshot>> GetHoldingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task AddActionAsync(ActionRecordBase action)
        {
            throw new NotImplementedException();
        }

        public Task<List<ActionRecordBase>> GetRecentActionsAsync(int take)
        {
            throw new NotImplementedException();
        }

        public Task<List<ActionRecordBase>> GetActionsByTypeAsync(ActionType type, int take)
        {
            throw new NotImplementedException();
        }

        public Task<List<ActionRecordBase>> GetAllActionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}