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

        [HttpPost("setprice")]
        public IActionResult SetPrice(SetPrice request)
        {
            OpResult result = _ledger.SetPrice(request.Name, request.Price);
            return OpResultMapper.ToActionResult(result);
        }
    }
}
