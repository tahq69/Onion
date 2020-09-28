using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Onion.Application.Interfaces;

namespace Onion.Infrastructure.Features.EmailFeatures.Commands
{
    /// <summary>
    /// Send email command.
    /// </summary>
    public class SendEmailCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets email recipient.
        /// </summary>
        public string To { get; set; } = null!;

        /// <summary>
        /// Gets or sets email subject.
        /// </summary>
        public string Subject { get; set; } = null!;

        /// <summary>
        /// Gets or sets email body message.
        /// </summary>
        public string Body { get; set; } = null!;

        /// <summary>
        /// Gets or sets email sender address (Configuration value used if is not provided).
        /// </summary>
        public string? From { get; set; }

        /// <summary>
        /// Send email command handler.
        /// </summary>
        public class SendEmailHandler : IRequestHandler<SendEmailCommand, Unit>
        {
            private readonly IEmailService _email;

            /// <summary>
            /// Initializes a new instance of the <see cref="SendEmailHandler"/> class.
            /// </summary>
            /// <param name="email">Email service.</param>
            public SendEmailHandler(IEmailService email)
            {
                _email = email;
            }

            /// <inheritdoc />
            public async Task<Unit> Handle(SendEmailCommand request, CancellationToken ct)
            {
                await _email.Send(request.To, request.Subject, request.Body, request.From);

                return Unit.Value;
            }
        }
    }
}