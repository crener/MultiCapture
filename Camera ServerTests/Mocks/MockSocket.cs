using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Hub.Networking;

namespace Camera_ServerTests.Mocks
{
    class MockSocket : ISocket
    {
        public MockSocket()
        {
            ReceiveData = new byte[] {};
            SendData = new byte[] {};
            ReceiveCount = 0;
            SlowDown = false;
        }

        public byte[] SendData { get; set; }
        public byte[] ReceiveData { get; set; }
        public int ReceiveCount { get; set; }
        public int RecieveQueryCount { get; set; }
        public bool SlowDown { get; set; }

        public void Connect(IPEndPoint endPoint)
        {
            Active = true;
        }

        public void Send(byte[] data)
        {
            SendData = data;
        }

        public int Receive(byte[] buffer)
        {
            if(SlowDown && RecieveQueryCount > 0) Thread.Sleep(1000);
            RecieveQueryCount++;
            if (ReceiveCount == ReceiveData.Length) return 0;

            //split into bits
            if (buffer.Length > ReceiveData.Length - ReceiveCount)
            {
                Array.Copy(ReceiveData, ReceiveCount, buffer, 0, ReceiveData.Length);
                ReceiveCount += ReceiveData.Length;
                return buffer.Length;
            }

            //send the all the data or remaining data
            int length = buffer.Length - ReceiveCount;
            Array.Copy(ReceiveData, ReceiveCount, buffer, 0, length);
            ReceiveCount = ReceiveData.Length;
            return length;
        }

        public bool Active { get; private set; }

        public void Shutdown(SocketShutdown shutdown)
        {
            Active = false;
        }

        public void Close()
        {
            Active = false;
        }

        public bool Connected { get; set; }
        public int Available { get; set; }
        public bool PollResult { get; set; }
        public bool Poll(int timout, SelectMode mode)
        {
            return PollResult;
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
            
        }

        public void Listen(int i)
        {
            
        }

        public ISocket Accept()
        {
            Active = true;
            return this;
        }
    }
}
