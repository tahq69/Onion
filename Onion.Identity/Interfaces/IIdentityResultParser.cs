using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Onion.Identity.Interfaces
{
    /// <summary>
    /// Identity result parser contract.
    /// </summary>
    public interface IIdentityResultParser
    {
        /// <summary>
        /// Converts identity result errors to Dictionary.
        /// </summary>
        /// <param name="result">Identity result.</param>
        /// <returns>Dictionary of the errors.</returns>
        IDictionary<string, ICollection<string>> Errors(IdentityResult result);
    }
}