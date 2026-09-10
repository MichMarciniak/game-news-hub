using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GameNewsHub.Api.Features.Auth;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }
    
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation($"EMAIL: {email}\nSUBJECT: {subject}\nMESSAGE: {htmlMessage}");
        return Task.CompletedTask;
    }
}