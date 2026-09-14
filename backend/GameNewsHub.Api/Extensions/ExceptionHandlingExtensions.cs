using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameNewsHub.Api.Extensions;

public static class ExceptionHandlingExtensions
{
    
    public static WebApplication UseAppExceptionHandler(this WebApplication app)
    {

        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;

                Log.Error(ex, "Unhandled exception on {Path}", context.Request.Path);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = 500,
                    Title = "An unexpected error occured",
                    Type = "Server.InternalError"
                });
            });
        });

        return app;
    }
}