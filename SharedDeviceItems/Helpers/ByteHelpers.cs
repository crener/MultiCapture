using System.IO;
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
            string fileName = location.Substring(location.LastIndexOf(Path.DirectorySeparatorChar));
            byte[] name = Encoding.ASCII.GetBytes(fileName + Constants.MessageSeperator),
                        file = File.ReadAllBytes(location),
                        eom = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            var messageData = new byte[name.Length + file.Length + eom.Length];
            name.CopyTo(messageData, 0);
            file.CopyTo(messageData, name.Length);
            eom.CopyTo(messageData, name.Length + file.Length);
            return messageData;
        }
    }
}
