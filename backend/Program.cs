using System.Text;
using backend.Configuration;
using backend.Data;
using backend.Extensions;
using backend.Models.Entities;
using backend.Services.Background;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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

// AUTH + OPENAPI
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthDocumentation();


builder.Services.AddControllers();


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