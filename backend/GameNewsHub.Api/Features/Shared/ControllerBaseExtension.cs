using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Shared;

public static class ControllerBaseExtension
{
    public static ObjectResult ProblemErr(this ControllerBase controller, Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _=> StatusCodes.Status500InternalServerError
        };

        return controller.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description
        );
    }
}