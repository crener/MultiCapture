using System.IO;
using System.Linq;
using System.Text;

namespace SharedDeviceItems.Helpers
{
    public static class ByteHelpers
    {
        /// <summary>
        /// Converts a file to bytes for sending accross a network
        /// </summary>
        /// <param name="location">path to the file</param>
        /// <returns>byte array that should be sent</returns>
        public static byte[] FileToBytes(string location)
        {
            string fileName = location.Substring(location.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            byte[] name = Encoding.ASCII.GetBytes(fileName + Constants.MessageSeparator),
                        file = File.ReadAllBytes(location),
                        eom = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            byte[] messageData = new byte[name.Length + file.Length + eom.Length];
            name.CopyTo(messageData, 0);
            file.CopyTo(messageData, name.Length);
            eom.CopyTo(messageData, name.Length + file.Length);
            return messageData;
        }

        /// <summary>
        /// Check if the end of message string is inside the data 
        /// </summary>
        /// <param before="data">the data array to parse</param>
        /// <param before="size">amount of data populated with valid data (starting from 0)</param>
        /// <returns>first byte location of the end of message</returns>
        public static int SearchEomIndex(byte[] data, int size)
        {
            if (size > data.Length) size = data.Length;

            for (int i = size - 1; i >= 0; i--)
            {
                if (data[i] == Constants.EndOfMessageBytes.Last())
                {
                    int i2 = i;
                    bool valid = true;
                    //last element has been found search for the lest of them
                    for (int u = Constants.EndOfMessageBytes.Length - 1; u >= 0; u--, i2--)
                    {
                        if (Constants.EndOfMessageBytes[u] != data[i2])
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid) return i2 + 1;
                }
            }

            return -1;
        }
    }
}
