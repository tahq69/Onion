namespace Onion.Domain.Settings
{
    /// <summary>
    /// Request correlation identifier middleware options.
    /// </summary>
    public class CorrelationIdOptions
    {
        /// <summary>
        /// The default header key value.
        /// </summary>
        public const string DefaultHeader = "CorrelationId";

        /// <summary>
        /// Gets or sets the header field name where the correlation ID will be
        /// stored.
        /// </summary>
        public string Header { get; set; } = DefaultHeader;

        /// <summary>
        /// Gets or sets a value indicating whether correlation ID is returned
        /// in the response headers.
        /// </summary>
        public bool IncludeInResponse { get; set; } = true;
    }
}