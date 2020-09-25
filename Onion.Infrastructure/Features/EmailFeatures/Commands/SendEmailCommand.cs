using MailKit.Net.Smtp;
using MailKit.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Onion.Domain.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Features.EmailFeatures.Commands
{
    public class SendEmailCommand : IRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }

        public class SendEmailHandler : IRequestHandler<SendEmailCommand>
        {
            private readonly MailSettings _settings;
            private readonly ILogger<SendEmailHandler> _logger;

            public SendEmailHandler(IOptions<MailSettings> settings, ILogger<SendEmailHandler> logger)
            {
                _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
                _logger = logger;
            }

            public async Task<Unit> Handle(SendEmailCommand request, CancellationToken ct)
            {
                try
                {
                    var email = new MimeMessage();
                    email.Sender = MailboxAddress.Parse(request.From ?? _settings.EmailFrom);
                    email.To.Add(MailboxAddress.Parse(request.To));
                    email.Subject = request.Subject;

                    var builder = new BodyBuilder();
                    builder.HtmlBody = request.Body;
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

                return Unit.Value;
            }
        }
    }
}