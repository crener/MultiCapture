using SharedDeviceItems;

namespace Hub.Networking
{
    internal interface INetwork
    {
        byte[] MakeRequest(ISocket socket, CameraRequest request);
    }

    public class StateObject
    {
        public ISocket WorkSocket = null;
        public byte[] Buffer = new byte[Constants.ByteArraySize];

        public int Saved = 0;
    }
}
