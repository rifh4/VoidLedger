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
        public async Task<IActionResult> Totals()
        {
            string report = await _ledger.BuildTotalsReportAsync();
            return Ok(new { report });
        }

        [HttpGet("actions/by-type")]
        public async Task<IActionResult> ActionsByType(string type, int take = 10)
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

            string report = await _ledger.BuildActionsByTypeReportAsync(actionType, take);
            return Ok(new { report });
        }
    }
}