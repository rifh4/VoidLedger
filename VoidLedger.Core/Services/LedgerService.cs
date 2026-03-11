using System.Text;
using VoidLedger.Core.Services;

namespace VoidLedger.Core
{
    public class LedgerService : ILedgerService
    {
        // Application-layer orchestrator: calls domain services + appends action log on success.
        private readonly ITradeService _tradeService;
        private readonly IClock _clock;

        // Persistence boundary (async). EF-backed implementation lives in Api.
        private readonly ILedgerStore _ledgerStore;

        public LedgerService(ITradeService tradeService, IClock clock, ILedgerStore ledgerStore)
        {
            _tradeService = tradeService;
            _clock = clock;
            _ledgerStore = ledgerStore;
        }

        public async Task<OpResult> DepositAsync(decimal amount)
        {
            if (amount <= 0)
                return new OpResult(false, ErrorCode.InvalidAmount, "Invalid deposit amount above 0.", null);

            decimal currentBalance = await _ledgerStore.GetBalanceAsync();
            decimal newBalance = currentBalance + amount;

            ActionRecordBase rec = new DepositAction(amount, _clock.UtcNow);

            await _ledgerStore.SetBalanceAsync(newBalance);
            await _ledgerStore.AddActionAsync(rec);
            await _ledgerStore.SaveChangesAsync();

            string msg = $"Deposited {Formatter.Money(amount)}. Balance: {Formatter.Money(newBalance)}";
            return new OpResult(true, ErrorCode.None, msg, rec);
        }

        public async Task<OpResult> SetPriceAsync(string name, decimal price)
        {
            string cleanName = (name ?? "").Trim().ToUpperInvariant();

            if (cleanName.Length == 0)
                return new OpResult(false, ErrorCode.InvalidName, "Name cannot be empty.", null);

            if (price <= 0)
                return new OpResult(false, ErrorCode.InvalidAmount, "Price must be above 0.", null);

            ActionRecordBase rec = new SetPriceAction(cleanName, price, _clock.UtcNow);

            await _ledgerStore.SetPriceAsync(cleanName, price);
            await _ledgerStore.AddActionAsync(rec);
            await _ledgerStore.SaveChangesAsync();

            string msg = $"Price for {cleanName} set to {Formatter.Money(price)}";
            return new OpResult(true, ErrorCode.None, msg, rec);
        }

        public async Task<OpResult> BuyAsync(string name, int qty)
        {
            OpResult result = await _tradeService.BuyAsync(name, qty);

            return result;
        }

        public async Task<OpResult> SellAsync(string name, int qty)
        {
            OpResult result = await _tradeService.SellAsync(name, qty);

            return result;
        }

        public async Task<string> BuildPortfolioReportAsync()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your Portfolio:");

            List<HoldingSnapshot> holdings = await _ledgerStore.GetHoldingsAsync();

            foreach (HoldingSnapshot holding in holdings)
            {
                decimal? maybePrice = await _ledgerStore.GetPriceAsync(holding.Name);

                if (maybePrice is not null)
                {
                    decimal value = maybePrice.Value * holding.Quantity;
                    sb.AppendLine($"{holding.Name}: {holding.Quantity} shares, Value: {Formatter.Money(value)}");
                }
                else
                {
                    sb.AppendLine($"{holding.Name}: {holding.Quantity} shares, Value: {Formatter.Money(0m)}");
                }
            }

            decimal balance = await _ledgerStore.GetBalanceAsync();
            sb.AppendLine($"Balance: {Formatter.Money(balance)}");

            return sb.ToString();
        }

        public async Task<string> BuildRecentActionsReportAsync(int n)
        {
            if (n < 1)
                return "No actions yet";

            List<ActionRecordBase> actions = await _ledgerStore.GetRecentActionsAsync(n);

            if (actions.Count == 0)
                return "No actions yet";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Your last {actions.Count} actions:");

            foreach (ActionRecordBase action in actions)
            {
                sb.AppendLine(action.Describe());
            }

            return sb.ToString();
        }

        public async Task<string> BuildActionsByTypeReportAsync(ActionType type, int n)
        {
            if (n < 1)
                return "Please enter a number greater than 0.";

            List<ActionRecordBase> actions = await _ledgerStore.GetActionsByTypeAsync(type, n);

            if (actions.Count == 0)
                return $"No actions of type {type} found.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Last {actions.Count} actions of type {type}:");

            foreach (ActionRecordBase action in actions)
            {
                sb.AppendLine(action.Describe());
            }

            return sb.ToString();
        }

        public async Task<string> BuildTotalsReportAsync()
        {
            List<ActionRecordBase> actions = await _ledgerStore.GetAllActionsAsync();

            if (actions.Count == 0)
                return "Nothing to report.";

            decimal totalDeposited = actions.OfType<DepositAction>().Sum(a => a.Amount);
            decimal totalSpentOnBuys = actions.OfType<BuyAction>().Sum(a => a.Total);
            decimal totalEarnedFromSells = actions.OfType<SellAction>().Sum(a => a.Total);
            decimal cashflow = totalDeposited - totalSpentOnBuys + totalEarnedFromSells;

            return $"\nTotals Report:" +
                   $"\nDeposited: {Formatter.Money(totalDeposited)}" +
                   $"\nSpent on buys: {Formatter.Money(totalSpentOnBuys)}" +
                   $"\nEarned from sells: {Formatter.Money(totalEarnedFromSells)}" +
                   $"\nNet cashflow: {Formatter.Money(cashflow)}.";
        }

    }
}