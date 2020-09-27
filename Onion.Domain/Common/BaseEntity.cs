namespace Onion.Domain.Common
{
    /// <summary>
    /// Application entity base implementation.
    /// </summary>
    public abstract class BaseEntity : IEntity<long>
    {
        /// <inheritdoc />
        public long Id { get; set; }
    }
}