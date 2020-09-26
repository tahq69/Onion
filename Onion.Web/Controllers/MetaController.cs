using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        /// <returns></returns>
        [HttpGet("/info")]
        public ActionResult<string> Info()
        {
            var assembly = typeof(Startup).Assembly;

            var lastUpdate = System.IO.File.GetLastWriteTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Ok($"Version: {version}, Last Updated: {lastUpdate}");
        }
    }
}