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
        public async Task<IActionResult> Buy(TradeRequest request)
        {
            OpResult result = await _ledger.BuyAsync(request.Name, request.Qty);
            return OpResultMapper.ToActionResult(result);
        }

        [HttpPost("sell")]
        public async Task<IActionResult> Sell(TradeRequest request)
        {
            OpResult result = await _ledger.SellAsync(request.Name, request.Qty);
            return OpResultMapper.ToActionResult(result);
        }
    }
}
