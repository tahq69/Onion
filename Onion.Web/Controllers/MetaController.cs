using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace Onion.Web.Controllers
{
    /// <summary>
    /// Application meta data controller.
    /// </summary>
    public class MetaController : BaseApiController
    {
        /// <summary>
        /// Application basic information.
        /// </summary>
        /// <returns>Application meta information.</returns>
        [HttpGet("/info")]
        public ActionResult<string> Info()
        {
            var assembly = typeof(Startup).Assembly;

            var lastUpdate = System.IO.File.GetLastWriteTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Ok(new
            {
                Version = version,
                LastUpdated = lastUpdate.ToString(CultureInfo.InvariantCulture),
            });
        }
    }
}