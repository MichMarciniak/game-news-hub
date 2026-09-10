using System.Net;
using backend.Configuration;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace GameNewsHub.Api.Features.Auth;

public class EmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public EmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }
    
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        
        var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = subject;

        message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();
        
        using var client = new SmtpClient();
        
        // zawszse sprawdź porty jakie zajmuje windows
        // bo mnie coś strzeli
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.None);
        
        if (!string.IsNullOrEmpty(_options.Username))
            await client.AuthenticateAsync(_options.Username, _options.Password);
        
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}