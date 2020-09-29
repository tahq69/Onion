using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Onion.Identity.Interfaces;

namespace Onion.Identity.Services
{
    /// <summary>
    /// Identity result parser implementation.
    /// </summary>
    public class IdentityResultParser : IIdentityResultParser
    {
        /// <inheritdoc/>
        public IDictionary<string, ICollection<string>> Errors(IdentityResult result)
        {
            var errors = new Dictionary<string, ICollection<string>>();
            foreach (IdentityError error in result.Errors.Where(e => e != null))
            {
                if (errors.ContainsKey(error.Code))
                    errors[error.Code].Add(error.Description);
                else
                    errors.Add(error.Code, new[] { error.Description });
            }

            return errors;
        }
    }
}