using System.Text;
using Hub.Networking;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public class SocketRequester : IRequester
    {
        private ISocket socket;

        public SocketRequester(ISocket socket)
        {
            this.socket = socket;
        }

        public byte[] Request(CameraRequest request)
        {
            return Request(Encoding.ASCII.GetBytes(((int)request).ToString()));
        }

        public byte[] Request(byte[] requestData)
        {
            if(!socket.Connected) throw new SocketNotConnectedException();

            //format the request data and send it to the camera
            {
                byte[] correct = InterconnectHelper.FormatSendData(requestData);
                socket.Send(correct);
            }

            //recieve data and return the result
            {
                byte[] buffer = new byte[Constants.HubBufferSize];
                int recieved = socket.Receive(buffer);

                return InterconnectHelper.RecieveData(buffer, recieved, socket);
            }
        }

        public int ClearSocket()
        {
            int count = 0;

            byte[] throwAway = new byte[8000];
            while (socket.Available > 0) count += socket.Receive(throwAway);

            return count;
        }
    }
}
