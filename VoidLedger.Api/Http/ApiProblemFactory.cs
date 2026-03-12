using Microsoft.AspNetCore.Mvc;

namespace VoidLedger.Api.Http
{
    internal static class ApiProblemFactory
    {
        internal static ActionResult InvalidTake(string detail)
        {
            ProblemDetails problemDetails = new ProblemDetails
            {
                Title = "InvalidTake",
                Detail = detail,
                Status = 400
            };

            problemDetails.Extensions["code"] = "InvalidTake";

            return new BadRequestObjectResult(problemDetails);
        }
    }
}
