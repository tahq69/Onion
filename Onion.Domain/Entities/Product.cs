using Onion.Domain.Common;

namespace Onion.Domain.Entities
{
    /// <summary>
    /// Product entity.
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Gets or sets product name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets product barcode.
        /// </summary>
        public string Barcode { get; set; } = null!;

        /// <summary>
        /// Gets or sets product description.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets product rate.
        /// </summary>
        public decimal Rate { get; set; }
    }
}