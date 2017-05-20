using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SharedDeviceItems.Networking;

namespace Hub_ClientTests.Desktop_Interconnect
{
    class MockUdpClient : IUdpClient
    {
        public Socket Client { get; set; }
        public int Available { get; set; }
        public byte[] recieveData { get; set; }
        public byte[] sentData { get; set; }

        public byte[] Receive(ref IPEndPoint remoteEP)
        {
            return recieveData;
        }

        public IAsyncResult BeginReceive(AsyncCallback requestCallback, object state)
        {
            return null;
        }

        public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
        {
            return recieveData;
        }

        public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
        {
            sentData = dgram;
            return bytes;
        }
    }
}
