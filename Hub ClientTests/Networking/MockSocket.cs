using System;
using System.Net;
using System.Net.Sockets;
using Hub.Networking;

namespace Hub_ClientTests.Networking
{
    class MockSocket : ISocket
    {
        public byte[] ReturnData { get; set; }
        public int FailCount { get; set; }
        public int maxSend { get; set; }
        public int byteCount { get; set; }


        public void Connect(IPEndPoint endPoint)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data)
        {

        }

        public int Receive(byte[] buffer)
        {
            if(buffer.Length < ReturnData.Length) maxSend = buffer.Length;

            if (FailCount > 0)
            {
                --FailCount;
                return 0;
            }

            if (maxSend > 0)
            {
                int length;
                if(ReturnData.Length - byteCount > maxSend) length = maxSend;
                else length = ReturnData.Length - byteCount;

                Array.Copy(ReturnData, byteCount, buffer, 0, length );
                byteCount += length;

                return length;
            }

            for (int index = 0; index < ReturnData.Length; index++)
                buffer[index] = ReturnData[index];
            return ReturnData.Length;
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
    }
}
