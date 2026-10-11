using GameNewsHub.Hosting.Options;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace GameNewsHub.Hosting.Logging;

public static class LoggingExtensions
{
    public static IHostApplicationBuilder AddAppLogging(this IHostApplicationBuilder builder, SeqOptions seqOptions )
    {
        var appName = builder.Environment.ApplicationName;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", appName)
            .WriteTo.Console()
            .WriteTo.File($"../logs/{appName}-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14)
            .WriteTo.Seq(seqOptions.Url)
            .CreateLogger();

        builder.Services.AddSerilog();

        return builder;
    }
}