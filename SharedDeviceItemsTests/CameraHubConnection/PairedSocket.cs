using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Hub.Networking;

namespace SharedDeviceItemsTests.CameraHubConnection
{
    class PairedSocket : ISocket
    {
        public byte[] RecieveData
        {
            get { return recieveData; }
            set
            {
                recieveData = value;
                RecievePosition = 0;
            }
        }

        public int RecievePosition { get; private set; }

        public bool DisconnectMidTransmission { get; set; }
        public bool OverridePollFalse { get; set; }
        public PairedSocket Socket { get; set; }
        public int SubDivideRecieveData { get; set; }


        private byte[] recieveData;

        public PairedSocket()
        {
            Connected = true;
            OverridePollFalse = true;
            SubDivideRecieveData = -1;
        }
        public PairedSocket(PairedSocket socket)
        {
            Connected = true;
            OverridePollFalse = true;
            Socket = socket;
            socket.Socket = this;
            SubDivideRecieveData = -1;
        }

        public void Connect(IPEndPoint endPoint)
        {
        }

        public void Send(byte[] data)
        {
            Socket.recieveData = data;
        }

        public int Receive(byte[] buffer)
        {
            if (recieveData == null)
            {
                do
                {
                    Thread.Sleep(20);
                } while (recieveData == null);
            }

            int length;
            if (SubDivideRecieveData != -1)
            {
                length = SubDivideRecieveData > recieveData.Length - RecievePosition
                    ? recieveData.Length - RecievePosition
                    : SubDivideRecieveData;
            }
            else
            {
                length = recieveData.Length - RecievePosition;
            }

            if (length > buffer.Length)
            {
                //send small chunk
                length = buffer.Length;
            }

            if (length == 0) throw new Exception("Tried to read data when there was none avaliable");
            if (DisconnectMidTransmission) Connected = false;

            Array.Copy(recieveData, RecievePosition, buffer, 0, length);
            RecievePosition += length;

            return length;
        }

        public void Shutdown(SocketShutdown shutdown)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool Connected { get; set; }

        public int Available
        {
            get
            {
                if (recieveData == null || !Connected) return 0;
                return recieveData.Length - RecievePosition;
            }
        }

        public bool Poll(int timout, SelectMode mode)
        {
            if (OverridePollFalse) return false;
            return Connected;
        }

        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback,
            object state)
        {
            throw new NotImplementedException();
        }

        public int EndReceive(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback,
            object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginConnect(EndPoint remoteEp, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void Bind(IPEndPoint localEndPoint)
        {
            throw new NotImplementedException();
        }

        public void Listen(int i)
        {
            throw new NotImplementedException();
        }

        public ISocket Accept()
        {
            return this;
        }
    }
}
