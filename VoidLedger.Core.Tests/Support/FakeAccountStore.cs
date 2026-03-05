namespace VoidLedger.Core.Tests.Support
{
    internal sealed class FakeAccountStore : IAccountStore
    {
        private decimal _balance = 0m;
        internal FakeAccountStore(decimal balance)
        {
            _balance = balance;
        }
        public  Task<decimal> GetBalanceAsync()
        {
            return Task.FromResult(_balance);
        }
        public Task SetBalanceAsync(decimal newBalance)
        {
            _balance = newBalance; 
            return Task.CompletedTask;
        }
    }
}
