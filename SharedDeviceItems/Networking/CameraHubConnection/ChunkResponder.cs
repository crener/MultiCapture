using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hub.Networking;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public class ChunkResponder : IResponder
    {
        private ISocket socket;
        private byte[] buffer = new byte[Constants.CameraBufferSize];
        private bool waitingForResponse = false;

        public ChunkResponder()
        {
        }

        public ChunkResponder(ISocket socket)
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


            //Inform the hub how large the data will be
            socket.Send(GenerateInformationPackage(data));

            int recieve = socket.Receive(buffer);
            byte[] command = InterconnectHelper.RecieveData(buffer, recieve, socket);
            if (command == Constants.EndTransferBytes) return;

            int chunkSize = int.Parse(Encoding.ASCII.GetString(command));

            int chunkAmount = data.Length / chunkSize;

            List<byte[]> chunks = new List<byte[]>(chunkAmount);

            for (int i = 0; i < chunkAmount; i++)
            {
                byte[] sampleChunks = new byte[chunkSize];
                Array.Copy(data, i * chunkSize, sampleChunks, 0, chunkSize);

                chunks.Add(sampleChunks);
            }

            if (data.Length % chunkSize != 0)
            {
                byte[] sampleChunks = new byte[data.Length % chunkSize];
                Array.Copy(data, chunkAmount * chunkSize, sampleChunks, 0, data.Length % chunkSize);

                chunks.Add(sampleChunks);
            }

            socket.Send(InterconnectHelper.FormatSendData(Constants.ReadyTransferBytes));

            int sendChunk;
            recieve = socket.Receive(buffer);
            command = InterconnectHelper.RecieveData(buffer, recieve, socket);

            while (!command.SequenceEqual(Constants.EndTransferBytes))
            {
                sendChunk = int.Parse(Encoding.ASCII.GetString(command));
                socket.Send(InterconnectHelper.FormatSendData(chunks[sendChunk]));

                recieve = socket.Receive(buffer);
                command = InterconnectHelper.RecieveData(buffer, recieve, socket);
            }

            waitingForResponse = false;
        }

        private byte[] GenerateInformationPackage(byte[] data)
        {
            return InterconnectHelper.FormatSendData(Encoding.ASCII.GetBytes(data.Length.ToString()));
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
            if (socket == null) return false;
            return Helpers.NetworkHelpers.Connected(socket);
        }
    }
}
