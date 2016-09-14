using System;
using System.Net;
using System.Net.Sockets;
using Hub.Networking;

namespace Hub.Helpers
{
    public static class Networking
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

        /// <summary>
        /// remove the unused data in the byte array
        /// </summary>
        /// <param name="data">data which will be sorted</param>
        /// <param name="dataUsed">the last position in the data that is still valid data</param>
        /// <returns>trimmed down data</returns>
        public static byte[] TrimExcessByteData(byte[] data, int dataUsed)
        {
            byte[] newData = new byte[dataUsed + 1];

            for (int i = 0; i <= dataUsed; i++)
            {
                newData[i] = data[i];
            }

            return newData;
        }

        /// <summary>
        /// remove the unused data in the byte array
        /// </summary>
        /// <param name="data">data which will be sorted</param>
        /// <returns>trimmed down data</returns>
        public static byte[] TrimExcessByteData(byte[] data)
        {
            //search for end of message string location
            int removeFrom = ByteManipulation.SearchEndOfMessageInt(data, data.Length);
            if (removeFrom == -1) throw new Exception("There must be an end of string message in the data");
            return TrimExcessByteData(data, removeFrom);
        }


        /// <summary>
        /// Check if the socket has disconnected
        /// </summary>
        /// <param name="s">socket</param>
        /// <returns>true if the socket is connected</returns>
        public static bool Connected(ISocket s)
        {
            if (s.Connected && s.Poll(1000, SelectMode.SelectRead) && s.Available == 0)
                return false;

            return true;
        }
    }
}
