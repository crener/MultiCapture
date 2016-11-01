using System.Net;
using System.Net.Sockets;

namespace SharedDeviceItems.Helpers
{
    public static class NetworkHelpers
    {
        /// <summary>
        /// Return the first IPv4 address that can be found
        /// Useful for testing the code works locally
        /// </summary>
        /// <returns>Valid IPv4 address</returns>
        public static IPAddress GrabIpv4()
        {
            return GrabIpv4(Dns.GetHostEntry(Dns.GetHostName()));
        }

        /// <summary>
        /// Return the first IPv4 address that can be found
        /// Useful for testing the code works locally
        /// </summary>
        /// <param name="ipHostInfo">IPHostEntry that will be tested</param>
        /// <returns>Valid IPv4 address</returns>
        public static IPAddress GrabIpv4(IPHostEntry ipHostInfo)
        {
            foreach (IPAddress item in ipHostInfo.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    return item;
                }
            }
            return ipHostInfo.AddressList[0];
        }
    }
}
