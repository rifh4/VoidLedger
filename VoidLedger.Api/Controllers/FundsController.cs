using Microsoft.AspNetCore.Mvc;
using VoidLedger.Core;
using VoidLedger.Api.Contracts;
using VoidLedger.Api.Http;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("deposit")]
    public sealed class FundsController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public FundsController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositRequest request)
        {
            OpResult result = await _ledger.DepositAsync(request.Amount);
            return OpResultMapper.ToActionResult(result);
        }
    }
}