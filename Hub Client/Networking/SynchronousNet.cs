using System;
using System.Text;
using Hub.Helpers;
using SharedDeviceItems;

namespace Hub.Networking
{
    class SynchronousNet : INetwork
    {
        private const int bufferSize = 400000;
        public byte[] MakeRequest(ISocket socket, CameraRequest request)
        {
            byte[] bytes = new byte[Constants.ByteArraySize],
                buffer = new byte[bufferSize];

            if (!socket.Connected) throw new Exception("Socket needs to be connnected");
            //if (!socket.Connected) socket.Connect();
            try
            {
                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes((int)request + Constants.EndOfMessage);
                socket.Send(msg);

                //grab the bytes
                bytes = new byte[Constants.ByteArraySize];
                int totalData = 0;
                while (true)
                {
                    int bytesRec = socket.Receive(buffer);
                    Array.Copy(buffer, 0, bytes, totalData, bytesRec);
                    totalData += bytesRec;
                    if (ByteManipulation.SearchEndOfMessage(bytes, totalData)) break;
                }

                return Helpers.Networking.TrimExcessByteData(bytes, totalData - 1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
