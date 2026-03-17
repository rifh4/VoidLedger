using Microsoft.AspNetCore.Mvc;
using VoidLedger.Api.Contracts;
using VoidLedger.Api.Http;
using VoidLedger.Core;
using VoidLedger.Core.Stores;

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

        [HttpGet]
        public async Task<ActionResult<List<PriceResponse>>> GetPrices()
        {
            List<PriceSnapshot> prices = await _ledger.GetPricesAsync();
            List<PriceResponse> response = prices.Select(ToPriceResponse).ToList();
            return Ok(response);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<PriceResponse>> GetPrice(string name)
        {
            PriceSnapshot? snapshot = await _ledger.GetPriceAsync(name);
            if (snapshot is null)
            {
                ProblemDetails pd = new ProblemDetails
                {
                    Title = "MissingPrice",
                    Detail = "Price not set",
                    Status = 404
                };

                pd.Extensions["code"] = "MissingPrice";
                return new ObjectResult(pd) { StatusCode = 404 };
            }


            PriceResponse response = ToPriceResponse(snapshot);
            return Ok(response);
        }

        // ChangeAmount and Direction are derived from the current and previous price.
        // If there is no previous price yet, change is null and direction is Unknown.
        private static PriceResponse ToPriceResponse(PriceSnapshot snapshot)
        {
            decimal? changeAmount = snapshot.PreviousPrice is null
                ? null
                : snapshot.Price - snapshot.PreviousPrice.Value;

            string direction = snapshot.PreviousPrice is null
                ? "Unknown"
                : snapshot.Price > snapshot.PreviousPrice.Value
                    ? "Up"
                    : snapshot.Price < snapshot.PreviousPrice.Value
                        ? "Down"
                        : "Flat";

            return new PriceResponse(
                snapshot.Name,
                snapshot.Price,
                snapshot.PreviousPrice,
                snapshot.UpdatedAtUtc,
                changeAmount,
                direction);
        }

    }
}
