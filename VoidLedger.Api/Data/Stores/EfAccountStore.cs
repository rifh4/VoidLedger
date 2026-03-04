using Microsoft.EntityFrameworkCore;
using VoidLedger.Api.Data.Entities;
using VoidLedger.Core;

namespace VoidLedger.Api.Data.Stores
{
    public class EfAccountStore : IAccountStore
    {

        private const int AccountId = 1;

        private readonly VoidLedgerDbContext _dbContext;

        public EfAccountStore(VoidLedgerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            AccountEntity account = await GetOrCreateAccountAsync();
            return account.Balance;
        }

        public async Task SetBalanceAsync(decimal newBalance)
        {
            AccountEntity account = await GetOrCreateAccountAsync();

            account.Balance = newBalance;

            await _dbContext.SaveChangesAsync();
        }

        private async Task<AccountEntity> GetOrCreateAccountAsync()
        {
            AccountEntity? account = await _dbContext.Accounts.SingleOrDefaultAsync();

            if (account != null)
            {
                return account;
            }

            account = new AccountEntity
            {
                Balance = 0m
            };

            _dbContext.Accounts.Add(account);

            // Save so the row actually exists in the database immediately.
            // (Not strictly required before returning, but it avoids edge cases later.)
            await _dbContext.SaveChangesAsync();

            return account;
        }
    }
}