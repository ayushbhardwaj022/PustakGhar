using E_ECOM_P.Services.ServiceInterfaces;
using E_EOM_Utility;   // if EmailSettings class inside Utility Folder
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

public class EmailService : IEmailservice
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmail(string to, string subject, string message)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_settings.FromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = message };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.PrimaryDomain, (_settings.PrimaryPort), false);
        await smtp.AuthenticateAsync(_settings.UsernameEmail, _settings.UsernamePassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
