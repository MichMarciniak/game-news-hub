using GameNewsHub.Api.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace GameNewsHub.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplication UseAppRequestLogging(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
            {
                await next();
            }
        });
        app.UseSerilogRequestLogging();
        return app;
    }


}
