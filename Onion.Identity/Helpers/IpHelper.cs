using System.Net;
using System.Net.Sockets;

namespace Onion.Identity.Helpers
{
    /// <summary>
    /// IP address helper.
    /// </summary>
    public class IpHelper
    {
        /// <summary>
        /// Gets host IP address.
        /// </summary>
        /// <returns>IP Address.</returns>
        public static string GetIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }
    }
}