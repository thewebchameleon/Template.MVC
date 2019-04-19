using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading.Tasks;
using Template.Infrastructure.Email.Contracts;
using Template.Infrastructure.Email.Models;

namespace Template.Infrastructure.Email
{
    public class EmailProvider : IEmailProvider
    {
        private readonly EmailSettings _emailSettings;

        public EmailProvider(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task Send(SendRequest request)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(request.FromAddress));
            message.To.Add(new MailboxAddress(request.ToAddress));
            message.Subject = request.Subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Body
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_emailSettings.Smtp.Server, _emailSettings.Smtp.Port, _emailSettings.Smtp.UseSsl);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(_emailSettings.Smtp.Username, _emailSettings.Smtp.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
