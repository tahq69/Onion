using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.XPath;
using Microsoft.Extensions.DependencyInjection;
using Onion.Shared.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Onion.Web.Extensions
{
    /// <summary>
    /// Swagger generator extension methods.
    /// </summary>
    public static class SwaggerGenExtensions
    {
        /// <summary>
        /// Loads the XML comments from type assembly.
        /// </summary>
        /// <param name="options">The swagger generator options.</param>
        /// <param name="asm">The assembly reference.</param>
        /// <returns>Updated swagger options.</returns>
        public static SwaggerGenOptions LoadXmlComments(this SwaggerGenOptions options, Assembly asm)
        {
            if (asm is null)
                throw new ArgumentNullException(nameof(asm));

            var name = asm.GetName().Name;
            var ordinal = StringComparison.OrdinalIgnoreCase;
            var manifest = asm
                .GetManifestResourceNames()
                .FirstOrDefault(resource => resource.Contains($"{name}.xml", ordinal));

            if (string.IsNullOrEmpty(manifest))
                return options;

            Stream? doc = asm.GetManifestResourceStream(manifest);
            options.IncludeXmlComments(() => new XPathDocument(doc), true);

            return options;
        }

        /// <summary>
        /// Loads the XML comments from provided type assembly.
        /// </summary>
        /// <param name="options">The swagger generator options.</param>
        /// <param name="assemblyType">Type reference to the assembly.</param>
        /// <returns>Updated swagger options.</returns>
        public static SwaggerGenOptions LoadXmlComments(this SwaggerGenOptions options, Type assemblyType)
        {
            if (assemblyType is null)
                throw new ArgumentNullException(nameof(assemblyType));

            return options.LoadXmlComments(assemblyType.Assembly);
        }

        /// <summary>
        /// Custom swagger element name generator.
        /// </summary>
        /// <param name="schema">The schema type.</param>
        /// <returns>Schema user friendly name.</returns>
        public static string SchemaIdSelector(Type schema) => schema.UserFriendlyName();
    }
}