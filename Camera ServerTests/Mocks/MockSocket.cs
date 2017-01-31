using System;
using System.Net;
using System.Net.Sockets;
using Hub.Networking;

namespace Camera_ServerTests.Mocks
{
    class MockSocket : ISocket
    {
        public byte[] sendData { get; set; }
        public byte[] receiveData { get; set; }
        public int receiveCount { get; set; }

        public void Connect(IPEndPoint endPoint)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data)
        {
            sendData = data;
        }

        public int Receive(byte[] buffer)
        {
            if(receiveCount == receiveData.Length) return 0;

            //slplit into bits
            if (buffer.Length > receiveData.Length - receiveCount)
            {
                Array.Copy(receiveData, receiveCount, buffer, 0, buffer.Length);
                receiveCount += buffer.Length;
                return buffer.Length;
            }

            //send the all the data or remaining data
            int length = buffer.Length - receiveCount;
            Array.Copy(receiveData, receiveCount, buffer, 0, length);
            receiveCount = receiveData.Length;
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
        public int Available { get; set; }
        public bool Poll(int timout, SelectMode mode)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
