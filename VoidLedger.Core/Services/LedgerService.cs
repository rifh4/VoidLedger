using System.Text;

namespace VoidLedger.Core
{
    public class LedgerService : ILedgerService
    {
        // Application-layer orchestrator: calls domain services + appends action log on success.
        private readonly Account _account;
        private readonly PriceBook _priceBook;
        private readonly Portfolio _portfolio;
        private readonly TradeService _tradeService;
        private readonly List<ActionRecordBase> _log;
        private readonly IClock _clock;

        public LedgerService(
            Account account,
            PriceBook priceBook,
            Portfolio portfolio,
            TradeService tradeService,
            List<ActionRecordBase> log,
            IClock clock)
        {
            _account = account;
            _priceBook = priceBook;
            _portfolio = portfolio;
            _tradeService = tradeService;
            _log = log;
            _clock = clock;
        }

        public OpResult SetPrice(string name, decimal price)
        {
            string cleanName = (name ?? "").Trim().ToUpperInvariant();
            if (cleanName.Length == 0)
                return new OpResult(false, ErrorCode.InvalidName, "Name cannot be empty.", null);

            bool ok = _priceBook.SetPrice(cleanName, price, out string msg);
            if (!ok)
            {
                // Map price validation to InvalidAmount; otherwise Unknown.
                ErrorCode code = price <= 0 ? ErrorCode.InvalidAmount : ErrorCode.Unknown;
                return new OpResult(false, code, msg, null);
            }

            ActionRecordBase rec = new SetPriceAction(cleanName, price, _clock.UtcNow);
            _log.Add(rec);
            return new OpResult(true, ErrorCode.None, msg, rec);
        }

        public OpResult Deposit(decimal amount)
        {
            bool ok = _account.Deposit(amount);
            if (!ok)
                return new OpResult(false, ErrorCode.InvalidAmount, "Invalid deposit amount above 0.", null);

            ActionRecordBase rec = new DepositAction(amount, _clock.UtcNow);
            _log.Add(rec);

            string msg = $"Deposited {Formatter.Money(amount)}. Balance: {Formatter.Money(_account.Balance)}";
            return new OpResult(true, ErrorCode.None, msg, rec);
        }

        public OpResult Buy(string name, int qty)
        {
            TradeResult tr = _tradeService.Buy(name, qty);
            if (!tr.Ok)
                return new OpResult(false, tr.Code, tr.Message, null);

            _priceBook.TryGetPrice(name, out decimal unitPrice);
            decimal total = unitPrice * qty;

            ActionRecordBase rec = new BuyAction(name, qty, unitPrice, total, _clock.UtcNow);
            _log.Add(rec);

            return new OpResult(true, ErrorCode.None, tr.Message, rec);
        }

        public OpResult Sell(string name, int qty)
        {
            TradeResult tr = _tradeService.Sell(name, qty);
            if (!tr.Ok)
                return new OpResult(false, tr.Code, tr.Message, null);

            _priceBook.TryGetPrice(name, out decimal unitPrice);
            decimal total = unitPrice * qty;

            ActionRecordBase rec = new SellAction(name, qty, unitPrice, total, _clock.UtcNow);
            _log.Add(rec);

            return new OpResult(true, ErrorCode.None, tr.Message, rec);
        }

        public string BuildPortfolioReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Your Portfolio:");

            foreach (var kvp in _portfolio.GetHoldings())
            {
                string name = kvp.Key;
                int qty = kvp.Value;

                if (_priceBook.TryGetPrice(name, out decimal price))
                {
                    decimal value = price * qty;
                    sb.AppendLine($"{name}: {qty} shares, Value: {Formatter.Money(value)}");
                }
                else
                {
                    sb.AppendLine($"{name}: {qty} shares, Value: {Formatter.Money(0m)}");
                }
            }

            sb.AppendLine($"Balance: {Formatter.Money(_account.Balance)}");
            return sb.ToString();
        }

        public string BuildRecentActionsReport(int n)
        {
            if (_log.Count == 0 || n < 1)
                return "No actions yet";

            if (n > _log.Count)
                n = _log.Count;

            var sb = new StringBuilder();
            sb.AppendLine($"Your last {n} actions:");
            int start = _log.Count - n;

            for (int i = start; i < _log.Count; i++)
                sb.AppendLine(_log[i].Describe());

            return sb.ToString();
        }

        public string BuildActionsByTypeReport(ActionType type, int n)
        {
            if (n < 1)
                return "Please enter a number greater than 0.";

            List<ActionRecordBase> items = _log
                .Where(a => a.Type == type)
                .OrderByDescending(a => a.At)
                .Take(n)
                .ToList();

            if (items.Count == 0)
                return $"No actions of type {type} found.";

            var sb = new StringBuilder();
            sb.AppendLine($"Last {items.Count} actions of type {type}:");
            foreach (var item in items)
                sb.AppendLine(item.Describe());

            return sb.ToString();
        }

        public string BuildTotalsReport()
        {
            decimal totalDeposited = _log.OfType<DepositAction>().Sum(a => a.Amount);
            decimal totalSpentOnBuys = _log.OfType<BuyAction>().Sum(a => a.Total);
            decimal totalEarnedFromSells = _log.OfType<SellAction>().Sum(a => a.Total);
            decimal cashflow = totalDeposited - totalSpentOnBuys + totalEarnedFromSells;

            if (_log.Count == 0)
                return "Nothing to report.";

            return $"\nTotals Report:" +
                   $"\nDeposited: {Formatter.Money(totalDeposited)}" +
                   $"\nSpent on buys: {Formatter.Money(totalSpentOnBuys)}" +
                   $"\nEarned from sells: {Formatter.Money(totalEarnedFromSells)}" +
                   $"\nNet cashflow: {Formatter.Money(cashflow)}.";
        }

        public string RunSmokeTests()
        {
            var sb = new StringBuilder();
            int pass = 0, fail = 0;

            void Check(string testName, bool condition, string failDetails)
            {
                if (condition)
                {
                    pass++;
                    sb.AppendLine($"PASS - {testName}");
                }
                else
                {
                    fail++;
                    sb.AppendLine($"FAIL - {testName} | {failDetails}");
                }
            }

            (Account acct, Dictionary<string, decimal> prices, Dictionary<string, int> holdings, List<ActionRecordBase> log, LedgerService ledger) NewSystem()
            {
                Account acct = new(0m);
                Dictionary<string, decimal> prices = new();
                Dictionary<string, int> holdings = new();
                List<ActionRecordBase> log = new();
                Portfolio portfolio = new(holdings);
                PriceBook priceBook = new(prices);
                TradeService trade = new(acct, priceBook, portfolio);
                IClock clock = new FixedClock(new DateTime(2026, 2, 26, 0, 0, 0, DateTimeKind.Utc));
                LedgerService ledger = new(acct, priceBook, portfolio, trade, log, clock);
                return (acct, prices, holdings, log, ledger);
            }

            {
                var sys = NewSystem();
                sys.ledger.Deposit(100m);
                decimal balBefore = sys.acct.Balance;
                int logBefore = sys.log.Count;

                var r = sys.ledger.Buy("ABC", 1);

                Check(
                    "Buy without price",
                    r.Ok == false
                    && r.Code == ErrorCode.MissingPrice
                    && r.Record == null
                    && sys.acct.Balance == balBefore
                    && !sys.holdings.ContainsKey("ABC")
                    && sys.log.Count == logBefore,
                    $"ok={r.Ok}, code={r.Code}, bal={sys.acct.Balance}, holdingsHasABC={sys.holdings.ContainsKey("ABC")}" +
                    $", logΔ={sys.log.Count - logBefore}, msg='{r.Message}'"
                );
            }

            {
                var sys = NewSystem();
                sys.ledger.SetPrice("ABC", 10m);
                decimal balBefore = sys.acct.Balance;
                int logBefore = sys.log.Count;

                var r = sys.ledger.Sell("ABC", 1);

                Check(
                    "Sell without holdings",
                    r.Ok == false
                    && r.Code == ErrorCode.MissingHolding
                    && r.Record == null
                    && sys.acct.Balance == balBefore
                    && !sys.holdings.ContainsKey("ABC")
                    && sys.log.Count == logBefore,
                    $"ok={r.Ok}, code={r.Code}, bal={sys.acct.Balance}, holdingsHasABC={sys.holdings.ContainsKey("ABC")}" +
                    $", logΔ={sys.log.Count - logBefore}, msg='{r.Message}'"
                );
            }

            {
                var sys = NewSystem();
                sys.ledger.SetPrice("ABC", 10m);
                sys.ledger.Deposit(100m);

                var buy = sys.ledger.Buy("ABC", 2);
                decimal balBefore = sys.acct.Balance;
                int logBefore = sys.log.Count;

                int qtyBefore = sys.holdings.TryGetValue("ABC", out int q0) ? q0 : 0;

                var r = sys.ledger.Sell("ABC", 3);

                bool hasAfter = sys.holdings.TryGetValue("ABC", out int qtyAfter);

                Check(
                    "Oversell",
                    buy.Ok == true
                    && buy.Code == ErrorCode.None
                    && buy.Record != null
                    && r.Ok == false
                    && r.Code == ErrorCode.Oversell
                    && r.Record == null
                    && sys.acct.Balance == balBefore
                    && hasAfter && qtyAfter == qtyBefore
                    && sys.log.Count == logBefore,
                    $"buyOk={buy.Ok}, buyCode={buy.Code}, sellOk={r.Ok}, sellCode={r.Code}, bal={sys.acct.Balance}" +
                    $", qtyBefore={qtyBefore}, qtyAfter={qtyAfter}, logΔ={sys.log.Count - logBefore}, msg='{r.Message}'"
                );
            }

            {
                var sys = NewSystem();
                sys.ledger.SetPrice("ABC", 10m);
                sys.ledger.Deposit(5m);

                decimal balBefore = sys.acct.Balance;
                int logBefore = sys.log.Count;

                var r = sys.ledger.Buy("ABC", 1);

                Check(
                    "Insufficient funds buy",
                    r.Ok == false
                    && r.Code == ErrorCode.InsufficientFunds
                    && r.Record == null
                    && sys.acct.Balance == balBefore
                    && !sys.holdings.ContainsKey("ABC")
                    && sys.log.Count == logBefore,
                    $"ok={r.Ok}, code={r.Code}, bal={sys.acct.Balance}, holdingsHasABC={sys.holdings.ContainsKey("ABC")}" +
                    $", logΔ={sys.log.Count - logBefore}, msg='{r.Message}'"
                );
            }

            {
                var sys = NewSystem();
                sys.ledger.SetPrice("ABC", 10m);
                sys.ledger.Deposit(100m);

                var buy = sys.ledger.Buy("ABC", 1);
                var sell = sys.ledger.Sell("ABC", 1);

                bool holdingRemoved = !sys.holdings.ContainsKey("ABC");
                bool balanceRestored = sys.acct.Balance == 100m;

                Check(
                    "Sell-to-zero removes holding",
                    buy.Ok == true && buy.Code == ErrorCode.None && buy.Record != null
                    && sell.Ok == true && sell.Code == ErrorCode.None && sell.Record != null
                    && holdingRemoved && balanceRestored,
                    $"buyOk={buy.Ok}, buyCode={buy.Code}, sellOk={sell.Ok}, sellCode={sell.Code}, holdingRemoved={holdingRemoved}" +
                    $", bal={sys.acct.Balance}, msg='{sell.Message}'"
                );
            }

            {
                var sys = NewSystem();
                sys.ledger.SetPrice("ABC", 10m);
                sys.ledger.SetPrice("ABC", 20m);
                sys.ledger.Deposit(100m);

                var r = sys.ledger.Buy("ABC", 1);

                bool usedNewPrice = sys.acct.Balance == 80m;

                Check(
                    "Set price overwrite works",
                    r.Ok == true
                    && r.Code == ErrorCode.None
                    && r.Record != null
                    && usedNewPrice,
                    $"ok={r.Ok}, code={r.Code}, bal={sys.acct.Balance}, msg='{r.Message}'"
                );
            }

            {
                var sys = NewSystem();
                int log0 = sys.log.Count;

                var failBuy = sys.ledger.Buy("ABC", 1);
                int logAfterFail = sys.log.Count;

                var okDep = sys.ledger.Deposit(10m);
                int logAfterOk = sys.log.Count;

                Check(
                    "Log increments only on success",
                    failBuy.Ok == false
                    && failBuy.Code == ErrorCode.MissingPrice
                    && failBuy.Record == null
                    && logAfterFail == log0
                    && okDep.Ok == true
                    && okDep.Code == ErrorCode.None
                    && okDep.Record != null
                    && logAfterOk == log0 + 1,
                    $"failOk={failBuy.Ok}, failCode={failBuy.Code}, log0={log0}, logAfterFail={logAfterFail}" +
                    $", okDepOk={okDep.Ok}, okDepCode={okDep.Code}, logAfterOk={logAfterOk}"
                );
            }

            sb.AppendLine();
            sb.AppendLine($"Smoke tests complete: {pass} PASS, {fail} FAIL");
            return sb.ToString();
        }
    }
}
