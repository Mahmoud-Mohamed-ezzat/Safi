using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Safi.Configuration;
using Safi.Dto.EmailDto;
using Safi.Interfaces;
namespace Safi.Repositories
{
    public class EmailRepository : IEmailService
    {
        private readonly EmailSettings _EmailSettings;
        private readonly ILogger<EmailRepository> _Logger;
        public EmailRepository(IOptions<EmailSettings> EmailSettings, ILogger<EmailRepository> Logger) {
            _EmailSettings = EmailSettings.Value;
            _Logger = Logger;
        }
        public async Task SendEmailAsync(SendEmailDto Request)
        {
            try { 
            var emailMessage = new MimeKit.MimeMessage();
                emailMessage.From.Add(new MimeKit.MailboxAddress(_EmailSettings.SenderName,_EmailSettings.SenderEmail));
                emailMessage.To.Add(new MimeKit.MailboxAddress("",Request.ToEmail));
                emailMessage.Subject=Request.Subject;
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = Request.Body
                };
                emailMessage.Body = bodyBuilder.ToMessageBody();
                var Client = new SmtpClient();
                Client.Connect(_EmailSettings.SmtpServer, _EmailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                Client.Authenticate(_EmailSettings.SenderEmail, _EmailSettings.SenderPassword);
                Client.Send(emailMessage);
                Client.Disconnect(true);
                _Logger.LogInformation($"Email sent successfully to {Request.ToEmail}");

            }
         catch(Exception ex) { 
            _Logger.LogError(ex, $"Failed to send email to {Request.ToEmail}: {ex.Message}");
            }
        }
    }
}
