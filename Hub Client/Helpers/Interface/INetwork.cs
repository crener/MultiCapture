using System;
using SharedDeviceItems;

namespace Hub.Networking
{
    internal interface INetwork
    {
        byte[] MakeRequest(CameraRequest request);
        byte[] MakeRequest(Byte[] requestData);
    }

    public class StateObject
    {
        public ISocket WorkSocket = null;
        public byte[] Buffer = new byte[Constants.ByteArraySize];

        public int Saved = 0;
    }
}
