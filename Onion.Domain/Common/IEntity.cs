namespace Onion.Domain.Common
{
    /// <summary>
    /// Database entity contract.
    /// </summary>
    /// <typeparam name="TKey">Type of the entity primary key.</typeparam>
    public interface IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets entity primary key identifier.
        /// </summary>
        TKey Id { get; set; }
    }
}