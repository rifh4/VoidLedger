using Microsoft.AspNetCore.Mvc;
using VoidLedger.Api.Contracts;
using VoidLedger.Core;
using VoidLedger.Core.Services.Models;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public PortfolioController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        // Legacy text endpoint kept for simple human-readable consumers.
        // API clients that need structured valuation data should use GET /portfolio/valuation.
        [HttpGet]
        public async Task<ActionResult<string>> GetPortfolio()
        {
            string report = await _ledger.BuildPortfolioReportAsync();
            return Ok(report);
        }

        [HttpGet("valuation")]
        public async Task<ActionResult<PortfolioValuationResponse>> GetPortfolioValuation()
        {
            PortfolioValuation valuation = await _ledger.GetPortfolioValuationAsync();
            List<PortfolioPositionResponse> positions = new();
            foreach (PortfolioPositionValuation p in valuation.Positions)
            {
                positions.Add(new PortfolioPositionResponse(p.Name, p.Quantity, p.CurrentPrice, p.PositionValue));
            }

            PortfolioValuationResponse response = new(positions, valuation.CashBalance, valuation.TotalPortfolioValue, valuation.TotalAccountValue);
            return Ok(response);

        }

    }
}
