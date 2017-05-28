using System;
using System.Net;
using System.Net.Sockets;

namespace SharedDeviceItems.Networking
{
    /// <summary>
    /// Wrapper for the standard C# Udp Client
    /// </summary>
    public class UdpClientWrapper : IUdpClient
    {
        public Socket Client { get { return client.Client; } }
        public int Available { get { return client.Available; } }

        private UdpClient client;

        public UdpClientWrapper()
        {
            client = new UdpClient();
        }

        public byte[] Receive(ref IPEndPoint remoteEP)
        {
            return client.Receive(ref remoteEP);
        }

        public IAsyncResult BeginReceive(AsyncCallback requestCallback, object state)
        {
            return client.BeginReceive(requestCallback, state);
        }

        public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
        {
            return client.EndReceive(asyncResult, ref remoteEP);
        }

        public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
        {
            return client.Send(dgram, bytes, endPoint);
        }
    }
}
