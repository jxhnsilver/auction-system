using AuctionSystem.Domain.Primitives;
using Microsoft.AspNetCore.Http;

namespace AuctionSystem.Presentation.Helpers
{
    public static class CustomResults
    {
        public static IResult Problem(Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Problem requires a failure result");

            var error = result.Error;

            if (error.Type == ErrorType.Unauthorized)
                return Results.Unauthorized();

            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var typeUri = error.Type switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            };

            return Results.Problem(
                title: error.Type.ToString(), 
                detail: error.Message, 
                type: typeUri, 
                statusCode: status);
        }

        public static IResult Problem<T>(Result<T> result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Problem requires a failure result");

            return Problem(Result.Failure(result.Error));
        }
    }
}
