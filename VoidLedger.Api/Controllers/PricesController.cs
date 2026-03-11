using Microsoft.AspNetCore.Mvc;
using VoidLedger.Api.Contracts;
using VoidLedger.Api.Http;
using VoidLedger.Core;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("prices")]
    public class PricesController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public PricesController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        [HttpPost]
        public async Task<IActionResult> SetPrice(SetPriceRequest request)
        {
            OpResult result = await _ledger.SetPriceAsync(request.Name, request.Price);
            return OpResultMapper.ToActionResult(result);
        }
    }
}
