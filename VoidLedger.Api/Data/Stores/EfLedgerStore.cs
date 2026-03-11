using Microsoft.EntityFrameworkCore;
using VoidLedger.Api.Data.Entities;
using VoidLedger.Core;
using VoidLedger.Core.Stores;

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

        public async Task<List<PriceSnapshot>> GetPricesAsync()
        {
            return await _dbContext.Prices
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new PriceSnapshot(p.Name, p.Price))
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddActionAsync(ActionRecordBase action)
        {
            AccountEntity account = await GetOrCreatePersistedDefaultAccountAsync();

            ActionLogEntity entity = action switch
            {
                DepositAction deposit => new ActionLogEntity
                {
                    AccountId = account.Id,
                    Type = deposit.Type,
                    AtUtc = deposit.At,
                    Amount = deposit.Amount
                },

                SetPriceAction setPrice => new ActionLogEntity
                {
                    AccountId = account.Id,
                    Type = setPrice.Type,
                    AtUtc = setPrice.At,
                    Name = setPrice.Name,
                    UnitPrice = setPrice.Price
                },

                BuyAction buy => new ActionLogEntity
                {
                    AccountId = account.Id,
                    Type = buy.Type,
                    AtUtc = buy.At,
                    Name = buy.Name,
                    Quantity = buy.Qty,
                    UnitPrice = buy.UnitPrice,
                    Total = buy.Total
                },

                SellAction sell => new ActionLogEntity
                {
                    AccountId = account.Id,
                    Type = sell.Type,
                    AtUtc = sell.At,
                    Name = sell.Name,
                    Quantity = sell.Qty,
                    UnitPrice = sell.UnitPrice,
                    Total = sell.Total
                },

                _ => throw new NotSupportedException($"Unsupported action type: {action.GetType().Name}")
            };

            _dbContext.ActionLogs.Add(entity);
        }

        private static ActionRecordBase MapAction(ActionLogEntity entity)
        {
            return entity.Type switch
            {
                ActionType.Deposit => new DepositAction(
                    entity.Amount ?? 0m,
                    entity.AtUtc),

                ActionType.SetPrice => new SetPriceAction(
                    entity.Name ?? "",
                    entity.UnitPrice ?? 0m,
                    entity.AtUtc),

                ActionType.Buy => new BuyAction(
                    entity.Name ?? "",
                    entity.Quantity ?? 0,
                    entity.UnitPrice ?? 0m,
                    entity.Total ?? 0m,
                    entity.AtUtc),

                ActionType.Sell => new SellAction(
                    entity.Name ?? "",
                    entity.Quantity ?? 0,
                    entity.UnitPrice ?? 0m,
                    entity.Total ?? 0m,
                    entity.AtUtc),

                _ => throw new NotSupportedException($"Unsupported action type: {entity.Type}")
            };
        }

        public async Task<List<ActionRecordBase>> GetRecentActionsAsync(int take)
        {
            AccountEntity account = await GetOrCreatePersistedDefaultAccountAsync();

            List<ActionLogEntity> rows = await _dbContext.ActionLogs
                .AsNoTracking()
                .Where(a => a.AccountId == account.Id)
                .OrderByDescending(a => a.AtUtc)
                .Take(take)
                .ToListAsync();

            rows.Reverse();

            List<ActionRecordBase> actions = rows
                .Select(MapAction)
                .ToList();

            return actions;
        }

        public async Task<List<ActionRecordBase>> GetActionsByTypeAsync(ActionType type, int take)
        {
            AccountEntity account = await GetOrCreatePersistedDefaultAccountAsync();

            List<ActionLogEntity> rows = await _dbContext.ActionLogs
                .AsNoTracking()
                .Where(a => a.AccountId == account.Id && a.Type == type)
                .OrderByDescending(a => a.AtUtc)
                .Take(take)
                .ToListAsync();

            rows.Reverse();

            List<ActionRecordBase> actions = rows
                .Select(MapAction)
                .ToList();

            return actions;
        }

        public async Task<List<ActionRecordBase>> GetAllActionsAsync()
        {
            AccountEntity account = await GetOrCreatePersistedDefaultAccountAsync();

            List<ActionLogEntity> rows = await _dbContext.ActionLogs
                .AsNoTracking()
                .Where(a => a.AccountId == account.Id)
                .OrderBy(a => a.AtUtc)
                .ToListAsync();

            List<ActionRecordBase> actions = rows
                .Select(MapAction)
                .ToList();

            return actions;
        }

    }
}