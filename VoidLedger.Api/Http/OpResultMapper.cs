using Microsoft.AspNetCore.Mvc;
using VoidLedger.Core;
namespace VoidLedger.Api.Http;


static class OpResultMapper
{
    private static int MapStatus(ErrorCode code)
    {
        if (code == ErrorCode.MissingPrice || code == ErrorCode.MissingHolding)
        {
            return 404;
        }
        else if (code == ErrorCode.Oversell || code == ErrorCode.InsufficientFunds)
        {
            return 409;
        }
        else if (code == ErrorCode.Unknown)
        {
            return 500;
        }
        else if (code == ErrorCode.None)
        {
            return 200;
        }
        else if (code == ErrorCode.InvalidName || code == ErrorCode.InvalidAmount )
        {
            return 400;
        }
        else
        {
            return 500;
        }
    }

    // Map service results to HTTP responses so controllers stay thin
    // and error responses use one ProblemDetails shape.
    public static IActionResult ToActionResult(OpResult result)
    {
        if (result.Ok == true)
        {
            return new OkObjectResult(
                new
                {
                    message = result.Message,
                    record = result.Record?.Describe()
                }
            );
        }

        int status = MapStatus(result.Code);

        ProblemDetails pd = new ProblemDetails
        {
            Title = result.Code.ToString(),
            Detail = result.Message,
            Status = status
        };

        pd.Extensions["code"] = result.Code.ToString();

        return new ObjectResult(pd) { StatusCode = status };


    }
}

