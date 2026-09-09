using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features;

[ApiController]
[Route("[controller]")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Problem();
        }

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
        {
            foreach (var (key, value) in firstError.Metadata)
            {
                problemDetails.Extensions[key] = value;
            }
        }

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}