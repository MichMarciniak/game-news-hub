using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using backend.Configuration;
using backend.Data;
using backend.Extensions;
using GameNewsHub.Api.Features;
using GameNewsHub.Api.Seeder;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "FrontendCorsPolicy";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// OPTIONS from appsettings and secrets
builder.Services.Configure<RecommendationWeights>(
    builder.Configuration.GetSection(RecommendationWeights.SectionName));

builder.Services.Configure<JwtTokenOptions>(
    builder.Configuration.GetSection(JwtTokenOptions.SectionName));

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(SmtpOptions.Section));

builder.Services.Configure<FrontendOptions>(
    builder.Configuration.GetSection(FrontendOptions.SectionName));

var devAuth = builder.Configuration.GetSection("DevAuth").Get<bool>();

//DATABSE + IDENTITY
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);
builder.Services.AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager();

builder.Services.AddHttpContextAccessor();

// bo .net jest tak piękny że nawet przy wczytywaniu tokenów
// zamiast "sub" jest jakiś schema...
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


// options narazie dla dev
builder.Services.Configure<IdentityOptions>(IdentityConfig.ConfigIdentity);

// CONFIG + SERVICES
builder.Services.AddFeatureServices();

// OPENAPI
builder.Services.AddAuthDocumentation();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://127.0.0.1:4200", "http://localhost:5180")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// AUTHENTICATION - Dev mode or normal
if (devAuth)
{
    builder.Services.AddAuthentication("DevScheme")
        .AddScheme<AuthenticationSchemeOptions, AuthDevHandler>("DevScheme", null);
    
    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder("DevScheme")
            .RequireAuthenticatedUser()
            .Build();
    });
}
else
{
    var tokenOptions = builder.Configuration.GetSection(JwtTokenOptions.SectionName).Get<JwtTokenOptions>();
    builder.Services.AddJwtAuthentication(tokenOptions!);
    builder.Services.AddAuthorization();
}


var app = builder.Build();

// DB MIGRATION
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    await RoleSeeder.SeedAsync(roleManager);

}

if (app.Environment.IsDevelopment())
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