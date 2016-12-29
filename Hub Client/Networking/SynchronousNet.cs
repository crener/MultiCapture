using System;
using System.Text;
using Hub.Helpers;
using SharedDeviceItems;

namespace Hub.Networking
{
    public class SynchronousNet : INetwork
    {
        private const int bufferSize = 400000;
        private ISocket socket;

        public SynchronousNet(ISocket socket)
        {
            this.socket = socket;
        }

        public byte[] MakeRequest(CameraRequest request)
        {
            return MakeRequest(Encoding.ASCII.GetBytes(((int)request).ToString()));
        }

        public byte[] MakeRequest(byte[] requestData)
        {
            byte[] bytes = new byte[Constants.ByteArraySize],
                buffer = new byte[bufferSize];

            if (!socket.Connected) throw new Exception("Socket needs to be connnected");
            try
            {
                // Encode the data string into a byte array.
                socket.Send(requestData);

                //get data size info
                int dataSize = -1;
                if (requestData != Encoding.ASCII.GetBytes(((int)CameraRequest.Alive).ToString()) &&
                    requestData != Encoding.ASCII.GetBytes(((int)CameraRequest.SetProporties).ToString()))
                {
                    int recSize = socket.Receive(buffer);
                    if (recSize > 0)
                    {
                        byte[] raw = new byte[recSize - Constants.EndOfMessage.Length];
                        Array.Copy(buffer, 0, raw, 0, raw.Length);
                        int.TryParse(Encoding.ASCII.GetString(raw), out dataSize);
                    }
                }

                bytes = new byte[Constants.ByteArraySize];
                int totalData = 0;
                do
                {
                    int bytesRec = socket.Receive(buffer);
                    Array.Copy(buffer, 0, bytes, totalData, bytesRec);
                    totalData += bytesRec;
                } while (totalData <= dataSize && !ByteManipulation.SearchEndOfMessage(bytes, totalData));

                return Helpers.Networking.TrimExcessByteData(bytes, totalData - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
    }
}
