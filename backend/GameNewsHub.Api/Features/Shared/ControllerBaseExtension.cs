using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Shared;

// TODO trzymaj się jakiegoś standardu
// bo raz jest problem.errors....
// a inny raz jest problem.details
// i we frontendzie nie wiadomo co wykorzystać
public static class ControllerBaseExtension
{
    public static ObjectResult ProblemErr(this ControllerBase controller, List<Error> errors)
    {
        if (errors.Count == 0) return controller.Problem();

        var firstError = errors[0];

        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = firstError.Description,
            Type = firstError.Code
        };

        if (firstError.Metadata != null)
            foreach (var (key, value) in firstError.Metadata)
                problemDetails.Extensions[key] = value;

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }

    public static ObjectResult ProblemErr(this ControllerBase controller, Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return controller.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description);
    }
}