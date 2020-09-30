namespace Onion.Domain.Settings
{
    /// <summary>
    /// Application email service setting.
    /// </summary>
    public class MailSettings
    {
        /// <summary>
        /// Gets or sets default email sender address.
        /// </summary>
        public string EmailFrom { get; set; } = null!;

        /// <summary>
        /// Gets or sets sMTP host name.
        /// </summary>
        public string SmtpHost { get; set; } = null!;

        /// <summary>
        /// Gets or sets sMTP port.
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Gets or sets sMTP service user name.
        /// </summary>
        public string SmtpUser { get; set; } = null!;

        /// <summary>
        /// Gets or sets sMTP service user password.
        /// </summary>
        public string SmtpPass { get; set; } = null!;

        /// <summary>
        /// Gets or sets email user display name.
        /// </summary>
        public string? DisplayName { get; set; }
    }
}