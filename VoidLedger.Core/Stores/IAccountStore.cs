namespace VoidLedger.Core;

    public interface IAccountStore
    {
        public Task<decimal> GetBalanceAsync();
        public Task SetBalanceAsync(decimal newBalance);
    }

