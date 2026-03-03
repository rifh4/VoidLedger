using Microsoft.AspNetCore.Mvc;
using VoidLedger.Core;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("actions")]
    public class ActionsController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public ActionsController(ILedgerService ledger)
        {
            _ledger = ledger;
        }

        [HttpGet("recent")]
        public IActionResult Recent([FromQuery] int take = 10)
        {
            if (take <= 0)
            {
                var pd = new ProblemDetails
                {
                    Title = "InvalidTake",
                    Detail = "The query parameter must be a positive integer.",
                    Status = 400
                };

                pd.Extensions["code"] = "InvalidTake";
                return new ObjectResult(pd) { StatusCode = 400 };
            }

            return Ok(new { report = _ledger.BuildRecentActionsReport(take) });
        }
    }
}
