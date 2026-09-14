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
    public static IHostBuilder AddAppLogging(this IHostBuilder host, SeqOptions seqOptions )
    {
        host.UseSerilog((ctx, cfg) => cfg
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14)
            .WriteTo.Seq(seqOptions.Url));

        return host;
    }

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
