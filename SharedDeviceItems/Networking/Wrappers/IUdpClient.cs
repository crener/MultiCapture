using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharedDeviceItems.Networking
{
    public interface IUdpClient
    {
        Socket Client { get; }
        int Available { get; }

        byte[] Receive(ref IPEndPoint remoteEP);
        IAsyncResult BeginReceive(AsyncCallback requestCallback, object state);
        byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP);
        int Send(byte[] dgram, int bytes, IPEndPoint endPoint);
    }
}
