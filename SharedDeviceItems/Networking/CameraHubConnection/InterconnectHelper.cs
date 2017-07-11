using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Hub.Networking;

    [assembly: InternalsVisibleTo("SharedDeviceItemsTests")]
namespace SharedDeviceItems.Networking.CameraHubConnection
{
    internal static class InterconnectHelper
    {
        /// <summary>
        /// Formats the send byte array so that it can be used to send successfully
        /// </summary>
        /// <param name="send">datat that needs to be sent</param>
        /// <returns>send bytes in correct format for sending</returns>
        public static byte[] FormatSendData(byte[] send)
        {
            byte[] formatted =
                new byte[send.Length + send.Length.ToString().Length + Constants.EndOfMessageBytes.Length];
            {
                int position = 0;
                byte[] temp = Encoding.ASCII.GetBytes(send.Length.ToString());

                Array.Copy(temp, 0, formatted, position, temp.Length);
                position += temp.Length;

                temp = Constants.EndOfMessageBytes;

                Array.Copy(temp, 0, formatted, position, temp.Length);
                position += temp.Length;

                Array.Copy(send, 0, formatted, position, send.Length);

                return formatted;
            }
        }


        public static byte[] RecieveData(byte[] buffer, int recieved, ISocket socket)
        {
            //figure out if that was all the data
            int end = Helpers.ByteHelpers.SearchEOMStartIndex(buffer, recieved);
            if (end < 0) throw new InvalidDataException("Recieved data does not have a size specification");

            int length;

            bool success = int.TryParse(Encoding.ASCII.GetString(buffer, 0, end), out length);
            if (!success) throw new InvalidDataException("Could not convert data length to number");

            byte[] output = new byte[length];

            if (length > buffer.Length - end - Constants.EndOfMessageBytes.Length)
            {
                //the data must be collected over multiple recieves
                int throwAwayData = end + Constants.EndOfMessageBytes.Length;
                int filled = buffer.Length - throwAwayData;
                Array.Copy(buffer, throwAwayData, output, 0, filled);

                while (filled < length)
                {
                    if (!Helpers.NetworkHelpers.Connected(socket)) throw new SocketDisconnectedException();

                    recieved = socket.Receive(buffer);
                    Array.Copy(buffer, 0, output, filled, recieved);
                    filled += recieved;
                }

                return output;
            }

            Array.Copy(buffer, end + Constants.EndOfMessageBytes.Length, output, 0, length);
            return output;
        }
    }
}
