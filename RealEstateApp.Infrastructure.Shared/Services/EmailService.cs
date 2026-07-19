using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RealEstateApp.Core.Application.DTOs.Email;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Settings;

namespace RealEstateApp.Infrastructure.Shared.Services;

public class EmailService : IEmailService
{
    private readonly MailSettings _mailSettings;

    public EmailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendAsync(EmailRequest request)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_mailSettings.EmailFrom ?? "");
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = request.Subject;

        var builder = new BodyBuilder { HtmlBody = request.Body };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
        
        // Connect and authenticate
        await smtp.ConnectAsync(_mailSettings.SmtpHost ?? "", _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_mailSettings.SmtpUser ?? "", _mailSettings.SmtpPass ?? "");
        
        // Send and disconnect
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}

