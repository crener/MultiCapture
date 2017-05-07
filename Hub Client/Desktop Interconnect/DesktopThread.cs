using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hub_Client.Util;

namespace Hub_Client.Desktop_Interconnect
{
    class DesktopThread
    {
        private readonly int DiscoveryPort = 8470;
        private readonly int DiscoveryResponsePort = 8471;
        private readonly int DesktopConnectionPort = 8472;
        private volatile bool connected = false;

        public DesktopThread()
        {
            IPEndPoint disc = new IPEndPoint(IPAddress.Any, DiscoveryPort);
            UdpClient udp = new UdpClient();
            Socket udpSocket = udp.Client;
            udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSocket.ExclusiveAddressUse = false;
            udpSocket.Bind(disc);
            UdpState state = new UdpState { client = udp, end = disc };
            udp.BeginReceive(DiscoveryAction, state);

            IPEndPoint desk = new IPEndPoint(IPAddress.Any, DesktopConnectionPort);
            TcpListener tcp = new TcpListener(desk);
            tcp.Start();
            tcp.BeginAcceptTcpClient(DesktopConnection, tcp);
        }


        void DiscoveryAction(IAsyncResult result)
        {
            if (connected) return;

            UdpState state = (UdpState)result.AsyncState;
            state.client.BeginReceive(DiscoveryAction, state);


            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);
            byte[] data = state.client.EndReceive(result, ref endpoint);
            Console.WriteLine("Discovery connection from: " + endpoint + ", message: " + Encoding.ASCII.GetString(data));

            byte[] response = Encoding.ASCII.GetBytes(Deployer.Inst.SysConfig.name);
            endpoint.Port = DiscoveryResponsePort;

            new UdpClient().Send(response, response.Length, endpoint);
        }

        void DesktopConnection(IAsyncResult result)
        {

        }


        private class UdpState
        {
            public UdpClient client { get; set; }
            public IPEndPoint end { get; set; }
        }
    }
}
