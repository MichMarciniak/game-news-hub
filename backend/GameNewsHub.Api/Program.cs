using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using GameNewsHub.Api.Configuration;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features;
using GameNewsHub.Api.Options;
using GameNewsHub.Api.Services;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb;
using GameNewsHub.External.Igdb.Options;
using GameNewsHub.External.Steam;
using GameNewsHub.External.Steam.Options;
using GameNewsHub.Hosting.Configuration;
using GameNewsHub.Hosting.Logging;
using GameNewsHub.Hosting.Options;
using GameNewsHub.Sync.Core;
using GameNewsHub.Sync.Core.Games;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
const string FrontendCorsPolicy = "FrontendCorsPolicy";

// OPTIONS from appsettings and secrets
services.AddValidatedOptions<SmtpOptions>(config);
services.AddValidatedOptions<FollowServiceOptions>(config);

var dbOptions = services.AddValidatedOptions<DatabaseOptions>(config);
var frontendOptions = services.AddValidatedOptions<FrontendOptions>(config);
var seqOptions = services.AddValidatedOptions<SeqOptions>(config);
var igdbOptions = services.AddValidatedOptions<IgdbOptions>(config);

var devAuth = config.GetValue<bool>("DevAuth");

//DATABSE + IDENTITY
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dbOptions.DefaultConnection)
);
services.AddIdentity<AppUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

services.AddHttpContextAccessor();

// options narazie dla dev
services.Configure<IdentityOptions>(IdentityConfig.ConfigIdentity);

// CONFIG + SERVICES
services.AddSteamServices(config);
services.AddIgdbServices(igdbOptions);
services.AddSyncCoreServices();
services.AddFeatureServices();
services.AddBackgroundServices();

// OPENAPI

services.AddOpenApi(options =>
{
    // enum as string
    options.AddSchemaTransformer((schema, context, _) =>
    {
        var targetType = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
        if (context.JsonTypeInfo.Type.IsEnum)
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = null;
            
            schema.Enum ??= new List<JsonNode>();
            schema.Enum.Clear();

            foreach (var enumName in Enum.GetNames(targetType))
            {
                schema.Enum.Add(JsonValue.Create(enumName));
            }
        }
        return Task.CompletedTask;
    });
    
    // convert query params to camelCase 
    options.AddOperationTransformer((operation, _, _) =>
    {
        if (operation.Parameters is null) return Task.CompletedTask;

        foreach (var parameter in operation.Parameters)
        {
            if (parameter is OpenApiParameter concrete
                && parameter.In == ParameterLocation.Query
                && !string.IsNullOrEmpty(parameter.Name))
            {
                concrete.Name = JsonNamingPolicy.CamelCase.ConvertName(parameter.Name);
            }
        }
        return Task.CompletedTask;
    });
});

services.AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute("application/json"));
    })
    .AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(frontendOptions!.Url, "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

services.Configure<RouteOptions>(opt =>
{
    opt.LowercaseUrls = true;
});

// AUTHENTICATION - Dev mode or normal
if (devAuth)
{
    services.AddAuthentication("DevScheme")
        .AddScheme<AuthenticationSchemeOptions, AuthDevHandler>("DevScheme", null);

    services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder("DevScheme")
            .RequireAuthenticatedUser()
            .Build();
    });
}
else
{
    services.AddAuthorization();
}

builder.AddAppLogging(seqOptions);

var app = builder.Build();

// logs + exceptions
app.UseAppExceptionHandler();
app.UseAppRequestLogging();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("localPc"))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwaggerUI(opt =>
        opt.SwaggerEndpoint("/openapi/v1.json", "GameNews Api")
    );
}

app.Lifetime.ApplicationStarted.Register(() =>
{
    var urlList = app.Urls;
    string urls = urlList.Count > 0 ? String.Join(" ", urlList) : "no urls";
    var scalarUrls = urlList.Count > 0 ? String.Join(" ", urlList.Select(u => $"{u}/scalar")) : "";

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"API URLs: {urls}");
    Console.WriteLine($"Scalar UI: {scalarUrls}");
    Console.ResetColor();
});

// app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

