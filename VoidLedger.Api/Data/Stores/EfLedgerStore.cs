using Microsoft.EntityFrameworkCore;
using VoidLedger.Api.Data.Entities;
using VoidLedger.Core;

namespace VoidLedger.Api.Data.Stores
{
    public sealed class EfLedgerStore : ILedgerStore
    {
        private readonly VoidLedgerDbContext _dbContext;

        public EfLedgerStore(VoidLedgerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private async Task<AccountEntity> GetOrCreateDefaultAccountAsync()
        {
            AccountEntity? localAccount = _dbContext.Accounts.Local.FirstOrDefault();

            if (localAccount is not null)
            {
                return localAccount;
            }
            AccountEntity? account = await _dbContext.Accounts.FirstOrDefaultAsync();

            if (account is not null)
            {
                return account;
            }
            account = new AccountEntity
            {
                Balance = 0m
            };
            _dbContext.Accounts.Add(account);
            return account;
        }

        private async Task<AccountEntity> GetOrCreatePersistedDefaultAccountAsync()
        {
            AccountEntity account = await GetOrCreateDefaultAccountAsync();

            if (account.Id == 0)
            {
                await _dbContext.SaveChangesAsync();
            }
            return account;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            AccountEntity account = await GetOrCreateDefaultAccountAsync();
            return account.Balance;
        }

        public async Task SetBalanceAsync(decimal newBalance)
        {
            AccountEntity accountEntity = await GetOrCreateDefaultAccountAsync();
            accountEntity.Balance = newBalance;
        }

        public async Task<decimal?> GetPriceAsync(string name)
        {
            PriceEntity? priceEntity = await _dbContext.Prices
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);

            if (priceEntity is null)
            {
                return null;
            }


            return priceEntity.Price;
        }

        public async Task SetPriceAsync(string name, decimal price)
        {
            PriceEntity? priceEntity = await _dbContext.Prices
                .FirstOrDefaultAsync(p => p.Name == name);

            if (priceEntity is null)
            {
                priceEntity = new PriceEntity
                {
                    Name = name,
                    Price = price
                };
                _dbContext.Prices.Add(priceEntity);
                return;
            }

            priceEntity.Price = price;
        }

        public async Task<int?> GetHoldingQuantityAsync(string name)
        {
            AccountEntity account = await GetOrCreatePersistedDefaultAccountAsync();

            HoldingEntity? holdingEntity = await _dbContext.Holdings
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.AccountId == account.Id && h.Name == name);

            if (holdingEntity is null)
            {
                return null;
            }
            return holdingEntity.Quantity;
        }

        public async Task SetHoldingQuantityAsync(string name, int quantity)
        {
            AccountEntity account = await GetOrCreatePersistedDefaultAccountAsync();

            HoldingEntity? holdingEntity = await _dbContext.Holdings
                .FirstOrDefaultAsync(h => h.AccountId == account.Id && h.Name == name);

            if (quantity == 0)
            {
                if (holdingEntity is not null)
                {
                    _dbContext.Holdings.Remove(holdingEntity);
                }

                return;
            }
            if (holdingEntity is null)
            {
                holdingEntity = new HoldingEntity
                {
                    AccountId = account.Id,
                    Name = name,
                    Quantity = quantity
                };

                _dbContext.Holdings.Add(holdingEntity);
                return;
            }
            holdingEntity.Quantity = quantity;
        }

        public async Task<List<HoldingSnapshot>> GetHoldingsAsync()
        {
            AccountEntity account = await GetOrCreatePersistedDefaultAccountAsync();

            List<HoldingSnapshot> holdings = await _dbContext.Holdings
                .AsNoTracking()
                .Where(h => h.AccountId == account.Id)
                .Select(h => new HoldingSnapshot(h.Name, h.Quantity))
                .ToListAsync();

            return holdings;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
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

    }
}