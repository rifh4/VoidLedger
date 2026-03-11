using Microsoft.AspNetCore.Mvc;
using VoidLedger.Core;

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

        [HttpGet]
        public async Task<ActionResult<string>> GetPortfolio()
        {
            string report = await _ledger.BuildPortfolioReportAsync();
            return Ok(report);
        }
    }
}
