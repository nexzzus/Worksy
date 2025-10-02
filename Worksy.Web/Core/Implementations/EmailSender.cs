using System.Net;
using System.Net.Mail;
using Worksy.Web.Core.Abstractions;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);
        var smtpUser = _config["Smtp:User"];
        var smtpPass = _config["Smtp:Pass"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage(smtpUser, email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mail);
    }
}