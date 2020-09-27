using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Onion.Infrastructure.Extensions
{
    /// <summary>
    /// C# collection type object extensions.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Convert input <see cref="IEnumerable{T}"/> to the <see cref="ICollection{T}"/> type object.
        /// </summary>
        /// <param name="src">The source collection.</param>
        /// <typeparam name="T">Type of the collection item.</typeparam>
        /// <returns>Converted collection.</returns>
        public static ICollection<T> ToCollection<T>(this IEnumerable<T> src) => src.ToList();
    }
}