using backend.Configuration;
using backend.Data;
using backend.Extensions;
using backend.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//DATABSE + IDENTITY
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
);

builder.Services.AddIdentity<AppUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(IdentityConfig.ConfigIdentity);


// CONFIG + SERVICES
builder.Services.Configure<ApiConfig>(
    builder.Configuration.GetSection("Api"));

builder.Services.AddIgdbServices(builder.Configuration);
builder.Services.AddDomainServices();

// OPENAPI
builder.Services.AddAuthDocumentation();

builder.Services.AddControllers();

// AUTHENTICATION - Dev mode or normal
if (builder.Environment.IsDevelopment())
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
    builder.Services.AddJwtAuthentication(builder.Configuration);
}


var app = builder.Build();

// DB MIGRATION
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwaggerUI(opt => 
        opt.SwaggerEndpoint("/openapi/v1.json", "GameNews Api")
    );
}

// app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();