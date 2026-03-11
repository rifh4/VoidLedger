using Microsoft.AspNetCore.Mvc;
using VoidLedger.Api.Contracts;
using VoidLedger.Api.Http;
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
        public async Task<IActionResult> Recent([FromQuery] int take = 10)
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

            List<ActionRecordBase> actions = await _ledger.GetRecentActionsAsync(take);

            List<ActionItemResponse> items = actions
                .Select(ActionItemMapper.Map)
                .ToList();

            RecentActionsResponse response = new(
                Count: items.Count,
                Items: items
            );

            return Ok(response);
        }
    }
}
