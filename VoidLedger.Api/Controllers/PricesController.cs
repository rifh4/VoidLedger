using Microsoft.AspNetCore.Mvc;
using VoidLedger.Api.Contracts;
using VoidLedger.Api.Http;
using VoidLedger.Core;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class PricesController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public PricesController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        [HttpPost("set-price")]
        public IActionResult SetPrice(SetPrice request)
        {
            OpResult result = _ledger.SetPrice(request.name, request.price);
            return OpResultMapper.ToActionResult(result);
        }
    }
}
