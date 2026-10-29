using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameNewsHub.Api.Extensions;

public static class ErrorOrExtension
{
    public static ActionResult<T> ToActionResult<T>(this ErrorOr<T> result, ControllerBase controller)
    {
        if (result.IsError)
        {
            return ToProblem(result.Errors, controller);
        }

        return controller.Ok(result.Value);
    }

    public static IActionResult ToNoContent<T>(this ErrorOr<T> result, ControllerBase controller)
    {
        if (result.IsError)
        {
            return ToProblem(result.Errors, controller);
        }

        return controller.NoContent();
    }

    private static ObjectResult ToProblem(List<Error> errors, ControllerBase controller)
    {
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }

            var validation = controller.ProblemDetailsFactory.CreateValidationProblemDetails(
                controller.HttpContext, modelState, StatusCodes.Status400BadRequest);

            return new ObjectResult(validation) { StatusCode = validation.Status };
        }

        var first = errors[0];
        var status = first.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = controller.ProblemDetailsFactory.CreateProblemDetails(
            controller.HttpContext, status, detail: first.Description);
        
        // opcjonalnie jakieś metadane do frontendu, np. retry after minutes
        
        return new ObjectResult(problem) { StatusCode = problem.Status };
    }
}