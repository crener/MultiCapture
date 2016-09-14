using System.Net.Sockets;
using SharedDeviceItems;

namespace Hub.Networking
{
    internal interface INetwork
    {
        byte[] MakeRequest(Socket socket, CameraRequest request);
    }

    public class StateObject
    {
        public Socket WorkSocket = null;
        public byte[] Buffer = new byte[Constants.ByteArraySize];

        public int Saved = 0;
    }
}
