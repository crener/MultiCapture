using System;
using System.Net.Sockets;
using Hub.Networking;

namespace Hub.Helpers
{
    public static class Networking
    {
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
                newData[i] = data[i];

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
            int removeFrom = ByteManipulation.SearchEndOfMessageIndex(data, data.Length) - 1;
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
