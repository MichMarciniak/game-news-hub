using System.IdentityModel.Tokens.Jwt;
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

var connectionString = config.GetConnectionString("DefaultConnection");

// OPTIONS from appsettings and secrets
services.AddValidatedOptions<SmtpOptions>(config);
services.AddValidatedOptions<FollowServiceOptions>(config);
services.AddValidatedOptions<RecommendationWeights>(config);

var frontendOptions = services.AddValidatedOptions<FrontendOptions>(config);
var seqOptions = services.AddValidatedOptions<SeqOptions>(config);

var devAuth = config.GetValue<bool>("DevAuth");

//DATABSE + IDENTITY
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);
services.AddIdentity<AppUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

services.AddHttpContextAccessor();

// options narazie dla dev
services.Configure<IdentityOptions>(IdentityConfig.ConfigIdentity);

// CONFIG + SERVICES
services.AddFeatureServices();
services.AddBackgroundServices();

// OPENAPI

services.AddOpenApi(options =>
{
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
            .WithOrigins(frontendOptions!.Url, "http://localhost:5173", "http://127.0.0.1:5173", "http://localhost:5180")
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
    services.AddCookieAuth();
    services.AddAuthorization();
}

builder.Host.AddAppLogging(seqOptions);

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

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n Scalar API: http://localhost:5190/scalar");
        Console.ResetColor();
    });
}

// app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();



app.Run();
