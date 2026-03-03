using Microsoft.AspNetCore.Mvc;
using VoidLedger.Core;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class PortfolioController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public PortfolioController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        [HttpGet("portfolio")]
        public async Task<IActionResult> GetPortfolio()
        {
            return Ok(new { report = _ledger.BuildPortfolioReport() });
        }
    }
}
