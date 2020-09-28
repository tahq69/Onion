using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Onion.Application.Interfaces;
using Onion.Domain.Settings;

namespace Onion.Infrastructure.Services
{
    /// <summary>
    /// Email service implementation.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly MailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="settings">Email service settings.</param>
        /// <param name="logger">Email service logger instance.</param>
        public EmailService(IOptions<MailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task Send(string to, string subject, string body, string? from = null)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(from ?? _settings.EmailFrom);
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not send E-Mail", ex);

                throw;
            }
        }
    }
}