using System;
using System.Text;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    class SocketRequester : IRequester
    {
        public byte[] Request(CameraRequest request)
        {
            return Request(Encoding.ASCII.GetBytes(((int)request).ToString()));
        }

        public byte[] Request(byte[] requestData)
        {
            throw new NotImplementedException();
        }
    }
}
