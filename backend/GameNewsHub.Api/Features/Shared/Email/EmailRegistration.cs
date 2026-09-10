using Microsoft.AspNetCore.Identity.UI.Services;

namespace GameNewsHub.Api.Features.Auth;

public static class EmailRegistration  
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        services.AddScoped<EmailTemplateService>();
        services.AddScoped<IEmailSender, EmailSender>();
        
        return services;
    }
}