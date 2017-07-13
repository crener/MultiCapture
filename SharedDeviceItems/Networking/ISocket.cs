using System;
using System.Net;
using System.Net.Sockets;

namespace Hub.Networking
{
    /// <summary>
    /// interface for the standard C# socket
    /// </summary>
    public interface ISocket
    {
        int ReceiveBufferSize { get; set; }
        int ReceiveTimeout { get; set; }
        void Connect(IPEndPoint endPoint);
        void Send(byte[] data);
        int Receive(byte[] buffer);
        void Shutdown(SocketShutdown shutdown);
        void Close();
        bool Connected { get; }
        int Available { get; }
        bool Poll(int timout, SelectMode mode);

        //Async methods
        IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags,
            AsyncCallback callback, object state);
        int EndReceive(IAsyncResult asyncResult);

        IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback,
            object state);

        IAsyncResult BeginConnect(EndPoint remoteEp, AsyncCallback callback, object state);
        void Bind(IPEndPoint localEndPoint);
        void Listen(int i);
        ISocket Accept();
    }
}
