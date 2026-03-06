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
        public async Task<IActionResult> SetPrice(SetPrice request)
        {
            OpResult result = _ledger.SetPrice(request.Name, request.Price);
            return OpResultMapper.ToActionResult(result);
        }
    }
}
