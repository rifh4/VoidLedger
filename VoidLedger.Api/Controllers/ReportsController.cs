using Microsoft.AspNetCore.Mvc;
using VoidLedger.Core;

namespace VoidLedger.Api.Controllers
{
    [ApiController]
    [Route("reports")]
    public class ReportsController : ControllerBase
    {
        private readonly ILedgerService _ledger;

        public ReportsController(ILedgerService ledger) => _ledger = ledger;

        [HttpGet("totals")]
        public IActionResult Totals()
            => Ok(new { report = _ledger.BuildTotalsReport() });

        [HttpGet("actions/by-type")]
        public IActionResult ActionsByType(string type, int take = 10)
        {
            if (take <= 0)
            {
                var pd = new ProblemDetails { Title = "InvalidTake", Detail = "The 'take' query parameter must be a positive integer.", Status = 400 };
                pd.Extensions["code"] = "InvalidTake";
                return new ObjectResult(pd) { StatusCode = 400 };
            }

            if (!Enum.TryParse<ActionType>(type, ignoreCase: true, out ActionType actionType))
            {
                var pd = new ProblemDetails
                {
                    Title = "InvalidActionType",
                    Detail = $"The 'type' query parameter must be one of: {string.Join(", ", Enum.GetNames<ActionType>())}.",
                    Status = 400
                };
                pd.Extensions["code"] = "InvalidActionType";
                return new ObjectResult(pd) { StatusCode = 400 };
            }

            return Ok(new { report = _ledger.BuildActionsByTypeReport(actionType, take) });
        }
    }
}