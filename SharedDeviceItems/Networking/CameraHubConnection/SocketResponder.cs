using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hub.Networking;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public class SocketResponder : IResponder
    {
        private ISocket socket;
        private byte[] buffer = new byte[Constants.CameraBufferSize];
        private bool waitingForResponse = false;

        public SocketResponder(ISocket socket)
        {
            this.socket = socket;
        }

        public void Connect()
        {
            socket.Accept();
            waitingForResponse = false;
        }

        public byte[] RecieveData()
        {
            if (!Connected()) throw new SocketNotConnectedException();
            if (waitingForResponse) throw new ResponseNeededException("There is a pending request in progress, complete it first");

            int recieved = socket.Receive(buffer);

            //figure out if that was all the data
            int end = Helpers.ByteHelpers.SearchEomIndex(buffer, recieved);
            if (end < 0) throw new InvalidDataException("Recieved data does not have a size specification");

            int length;

            bool success = int.TryParse(Encoding.ASCII.GetString(buffer, 0, end), out length);
            if (!success) throw new InvalidDataException("Could not convert data length to number");

            byte[] output = new byte[length];

            if (length > buffer.Length)
            {
                //the data must be collected over multiple recieves
                int throwAwayData = end + Constants.EndOfMessageBytes.Length;
                int filled = buffer.Length - throwAwayData;
                Array.Copy(buffer, throwAwayData, output, 0, filled);

                while (filled < length)
                {
                    if (!Connected()) throw new SocketDisconnectedException();

                    recieved = socket.Receive(buffer);
                    Array.Copy(buffer, 0, output, filled, recieved);
                    filled += recieved;
                }

                waitingForResponse = true;
                return output;
            }

            waitingForResponse = true;
            Array.Copy(buffer, end + Constants.EndOfMessageBytes.Length, output, 0, length);
            return output;
        }

        public void SendResponse(byte[] data)
        {
            if (!Connected()) throw new SocketNotConnectedException();

            //format the data so that it can be sent off
            byte[] formatted = new byte[data.Length + data.Length.ToString().Length + Constants.EndOfMessageBytes.Length];
            {
                int position = 0;
                byte[] temp = Encoding.ASCII.GetBytes(data.Length.ToString());

                Array.Copy(temp, 0, formatted, position, temp.Length);
                position += temp.Length;

                temp = Constants.EndOfMessageBytes;

                Array.Copy(temp, 0, formatted, position, temp.Length);
                position += temp.Length;

                Array.Copy(data, 0, formatted, position, data.Length);
            }

            waitingForResponse = false;
            socket.Send(formatted);
        }

        public int ClearSocket()
        {
            int count = 0;

            byte[] throwAway = new byte[8000];
            while (socket.Available > 0) count += socket.Receive(throwAway);

            return count;
        }

        public bool Connected()
        {
            return Connected(socket);
        }

        /// <summary>
        /// test if the socket is connected to a device
        /// </summary>
        /// <param name="s">socket that needs to be tested</param>
        /// <returns>true if connected to a client</returns>
        private bool Connected(ISocket s)
        {
            if (!s.Connected || s.Poll(1000, SelectMode.SelectRead) && s.Available == 0)
                return false;

            return true;
        }
    }
}
