using System;
using System.Net;
using System.Net.Sockets;
using Hub.Networking;
using SharedDeviceItems;
using System.Text;

namespace Hub_ClientTests.Networking
{
    class MockSocket : ISocket
    {
        public byte[] ReturnData { get; set; }
        public int FailCount { get; set; }
        public int maxSend { get; set; }
        public int byteCount { get; set; }

        public int recieveCount { get; set; }


        public void Connect(IPEndPoint endPoint)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data)
        {

        }

        public int Receive(byte[] buffer)
        {
            if (FailCount > 0)
            {
                --FailCount;
                return 0;
            }

            byte[] preData = null;
            int offset = 0;
            if(recieveCount < 1)
            {
                ++recieveCount;
                preData = Encoding.ASCII.GetBytes(ReturnData.Length + Constants.EndOfMessage);
                Array.Copy(preData, offset, buffer, 0, preData.Length);
                offset = preData.Length;
            }

            if(buffer.Length < ReturnData.Length + offset) maxSend = buffer.Length;

            //chop up the return data into smaller segments
            if (maxSend > 0)
            {
                int length;
                if(ReturnData.Length - byteCount + offset > maxSend) length = maxSend - offset;
                else length = ReturnData.Length - byteCount - offset;

                Array.Copy(ReturnData, byteCount, buffer, offset, length );
                byteCount += length;

                return length + offset;
            }

            //return all of the data (since it doesn't need to be split up)
            Array.Copy(ReturnData, byteCount, buffer, offset, ReturnData.Length);
            byteCount += ReturnData.Length;
            return ReturnData.Length + offset;
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
