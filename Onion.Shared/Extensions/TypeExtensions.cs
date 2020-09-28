using System;
using System.Linq;
using System.Text;

namespace Onion.Shared.Extensions
{
    /// <summary>
    /// Extensions for the c# <see cref="Type"/> object.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Generate user friendly name of the type.
        /// </summary>
        /// <param name="type">The type to generate name for.</param>
        /// <returns>User friendly name of the type.</returns>
        /// <example>
        /// <code>
        ///     string name = typeof(A.B.C.GenericType&lt;System.String&gt;).UserFriendlyName();
        ///     Assert.Equal("A.B.C.GenericType&lt;String&gt;", name);
        /// </code>
        /// </example>
        public static string UserFriendlyName(this Type type)
        {
            const StringComparison compare = StringComparison.InvariantCultureIgnoreCase;

            if (!type.IsGenericType)
                return type.Name;

            StringBuilder sb = new StringBuilder();

            sb.Append(type.Name.Substring(0, type.Name.LastIndexOf("`", compare)));
            sb.Append(type.GetGenericArguments().Aggregate(
                "<",
                (a, t) => a + (a == "<" ? string.Empty : ",") + t.UserFriendlyName()));
            sb.Append(">");

            return sb.ToString();
        }

        /// <summary>
        /// Generate user friendly name with namespace of the provided type.
        /// </summary>
        /// <param name="type">The type to generate name for.</param>
        /// <param name="discardPrefix">Namespace part to be cut off.</param>
        /// <returns>User friendly name of the type.</returns>
        /// <example>
        /// <code>
        ///     string name = typeof(A.B.C.GenericType&lt;System.String&gt;).UserFriendlyName("A.B");
        ///     Assert.Equal("C.GenericType&lt;String&gt;", name);
        /// </code>
        /// </example>
        public static string UserFriendlyName(this Type type, string discardPrefix)
        {
            const StringComparison compare = StringComparison.InvariantCultureIgnoreCase;

            string Name(Type t)
            {
                string? nameSpace = type.Namespace;
                if (!string.IsNullOrWhiteSpace(discardPrefix))
                {
                    nameSpace = type.Namespace?.Replace(discardPrefix, string.Empty, compare);
                }

                if (type.Namespace?.StartsWith("System") ?? false)
                {
                    nameSpace = null;
                }

                string prefix = nameSpace is null ? string.Empty : $"{nameSpace}.";

                if (prefix.StartsWith(".")) prefix = prefix.Substring(1, prefix.Length - 1);

                return $"{prefix}{t.Name}";
            }

            if (!type.IsGenericType)
                return Name(type);

            StringBuilder sb = new StringBuilder();

            var fullName = Name(type);
            sb.Append(fullName.Substring(0, fullName.LastIndexOf("`", compare)));
            sb.Append(type.GetGenericArguments().Aggregate(
                "<",
                (a, t) => a + (a == "<" ? string.Empty : ",") + t.UserFriendlyName(discardPrefix)));
            sb.Append(">");

            return sb.ToString();
        }
    }
}