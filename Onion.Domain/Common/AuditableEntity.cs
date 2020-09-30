using System;

namespace Onion.Domain.Common
{
    /// <summary>
    /// Auditable entity base implementation.
    /// </summary>
    public abstract class AuditableEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets user account email created this record.
        /// </summary>
        public string CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets date of record creations.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets user account email las modified this record.
        /// </summary>
        public string ModifiedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets date of record last modification.
        /// </summary>
        public DateTimeOffset ModifiedAt { get; set; }
    }
}