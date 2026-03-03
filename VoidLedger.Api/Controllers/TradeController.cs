using Microsoft.AspNetCore.Mvc;
using VoidLedger.Api.Contracts;
using VoidLedger.Api.Http;
using VoidLedger.Core;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("trade")]
    public class TradeController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public TradeController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Trade(TradeRequest request)
        {
            OpResult result = _ledger.Buy(request.Name, request.Qty);
            return OpResultMapper.ToActionResult(result);
        }

        [HttpPost("sell")]
        public async Task<IActionResult> Sell(TradeRequest request)
        {
            OpResult result = _ledger.Sell(request.Name, request.Qty);
            return OpResultMapper.ToActionResult(result);
        }
    }
}
