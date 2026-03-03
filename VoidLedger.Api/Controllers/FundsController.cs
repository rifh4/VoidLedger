using Microsoft.AspNetCore.Mvc;
using VoidLedger.Core;
using VoidLedger.Api.Contracts;
using VoidLedger.Api.Http;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("")]
    public sealed class FundsController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public FundsController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositRequest request)
        {
            OpResult result = _ledger.Deposit(request.Amount);
            return OpResultMapper.ToActionResult(result);
        }
    }
}


