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

        byte[] bytes = new byte[Constants.ByteArraySize],
            buffer = new byte[bufferSize];
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
            int recSize = 0;
#if DEBUG
            string requestStr = Encoding.ASCII.GetString(requestData);
#endif

            if (!socket.Connected) throw new Exception("Socket needs to be connected");
            try
            {
                // Encode the data string into a byte array and send to the camera
                socket.Send(requestData);

                //these need to be initialised early encase there is a data length
                bytes = new byte[Constants.ByteArraySize];
                int totalData = 0;

                //get data size info
                int dataSize = -1;
                if (ExpectsSize(requestData))
                {
                    while (recSize <= 0) recSize = socket.Receive(buffer);

                    byte[] raw = new byte[recSize - Constants.EndOfMessage.Length];
                    Array.Copy(buffer, 0, raw, 0, raw.Length);
#if DEBUG
                    string bufferStr = Encoding.ASCII.GetString(buffer);
                    string rawStr = Encoding.ASCII.GetString(raw);
#endif
                    int indexData = ByteManipulation.SearchEndOfMessageStartIndex(buffer, recSize);
                    byte[] sizeData = new byte[indexData];
                    Array.Copy(buffer, 0, sizeData, 0, sizeData.Length);
#if DEBUG
                    string sizeStr = Encoding.ASCII.GetString(sizeData);
                    if (!int.TryParse(sizeStr, out dataSize)) Console.WriteLine("Failed to extract image size");
#else
                    if (!int.TryParse(Encoding.ASCII.GetString(sizeData), out dataSize)) Console.WriteLine("Failed to extract image size");
#endif
                    else
                    {
                        int expectedSize = recSize - indexData - Constants.EndOfMessage.Length;
                        int copyStart = indexData + Constants.EndOfMessage.Length;
                        Array.Copy(buffer, copyStart, bytes, totalData, expectedSize);
                        totalData = expectedSize;
                    }
                }

                bool pass = dataSize > 0 && totalData < dataSize;
                if (!pass) pass = !ByteManipulation.SearchEndOfMessage(bytes, totalData);
                while (pass)
                {
                    int bytesRec = socket.Receive(buffer);
                    Array.Copy(buffer, 0, bytes, totalData, bytesRec);
                    totalData += bytesRec;

                    pass = dataSize > 0 && totalData < dataSize;
                    if (pass) pass = !ByteManipulation.SearchEndOfMessage(bytes, totalData);
                }

#if DEBUG
                byte[] preReturn = Helpers.Networking.TrimExcessByteData(bytes, totalData - 1);
                string imageData = Encoding.ASCII.GetString(preReturn);
                return preReturn;
#else
                return Helpers.Networking.TrimExcessByteData(bytes, totalData -1);
#endif
            }
            catch (ArithmeticException e)
            {
                Console.WriteLine(e);
                throw e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        /// <summary>
        /// The request that is being processed expects a size to be returned
        /// </summary>
        /// <returns></returns>
        private bool ExpectsSize(byte[] request)
        {
            string rawRequest = Encoding.ASCII.GetString(request);
            int seperatorLocation = rawRequest.IndexOf(Constants.ParamSeparator);
            if (seperatorLocation <= 0) return true;
            string requestId = rawRequest.Substring(0, seperatorLocation);

            if (requestId == ((int)CameraRequest.Alive).ToString()) return false;
            if (requestId == ((int)CameraRequest.SetProporties).ToString()) return false;

            return true;
        }
    }
}
