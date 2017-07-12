using Hub.Networking;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public class SocketResponder : IResponder
    {
        private ISocket socket;
        private byte[] buffer = new byte[Constants.HubBufferSize];
        private bool waitingForResponse = false;

        public SocketResponder()
        {
        }
        public SocketResponder(ISocket socket)
        {
            this.socket = socket;
        }

        public void Connect(ISocket listeningSocket)
        {
            socket = listeningSocket.Accept();
            waitingForResponse = false;
        }

        public void Disconnect()
        {
            if (!Connected()) throw new SocketNotConnectedException();
            socket.Close();
        }

        public byte[] RecieveData()
        {
            if (!Connected()) throw new SocketNotConnectedException();
            if (waitingForResponse) throw new ResponseNeededException("There is a pending request in progress, complete it first");

            int recieved = socket.Receive(buffer);

            waitingForResponse = true;

            return InterconnectHelper.RecieveData(buffer, recieved, socket);
        }

        public void SendResponse(byte[] data)
        {
            if (!Connected()) throw new SocketNotConnectedException();

            //format the data so that it can be sent off
            byte[] formatted = InterconnectHelper.FormatSendData(data);

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
            if(socket == null) return false;
            return Helpers.NetworkHelpers.Connected(socket);
        }
    }
}
