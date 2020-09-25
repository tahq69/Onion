using System;

namespace Onion.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}