using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hub.Util;

namespace Hub.DesktopInterconnect
{
    static class DesktopThread
    {
        private static readonly int DiscoveryPort = 8470;
        private static readonly int DiscoveryResponsePort = 8471;
        private static readonly int DesktopConnectionPort = 8472;
        private static volatile bool connected = false;
        private static volatile bool started = false;

        private static DesktopConnection connection;
        private static UdpClient udp;
        private static TcpListener tcpListener;

        public static void Start()
        {
            if (started) return;
            started = true;

            //Desktop device discovery - external method to find device on the network
            IPEndPoint disc = new IPEndPoint(IPAddress.Any, DiscoveryPort);
            udp = new UdpClient();
            Socket udpSocket = udp.Client;
            udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSocket.ExclusiveAddressUse = false;
            udpSocket.Bind(disc);
            udp.BeginReceive(DiscoveryAction, udp);

            //Desktop device connection - direct interaction with device (stops D3)
            IPEndPoint desk = new IPEndPoint(IPAddress.Any, DesktopConnectionPort);
            tcpListener = new TcpListener(desk);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(DesktopConnection, tcpListener);
        }


        static void DiscoveryAction(IAsyncResult result)
        {
            if (connected) return;

            UdpClient state = (UdpClient)result.AsyncState;
            state.BeginReceive(DiscoveryAction, state);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);

            byte[] data = state.EndReceive(result, ref endpoint);
            Console.WriteLine("Discovery connection from: " + endpoint + ", message: " + Encoding.ASCII.GetString(data));

            byte[] response = Encoding.ASCII.GetBytes(Deployer.Inst.SysConfig.name);
            endpoint.Port = DiscoveryResponsePort;

            state.Send(response, response.Length, endpoint);
        }


        static void DesktopConnection(IAsyncResult result)
        {
            TcpListener listener = (TcpListener)result.AsyncState;
            TcpClient tcp = listener.EndAcceptTcpClient(result);

            Console.WriteLine("Desktop connection from " + tcp.Client.RemoteEndPoint);

            if (connected)
            {
                Console.WriteLine("Already connected, ignoring request!");
                return;
            }

            if (tcp.Connected)
            {
                //ensure that something will be able to handle the requests
                connection = new DesktopConnection(tcp);
                connected = true;
                Console.WriteLine("Connection Successful");
            }
        }

        public static void Disconnected()
        {
            Console.WriteLine("Desktop connection has been terminated");
            connected = false;
            connection = null;

            //clear out old requests
            if (udp.Available > 0)
            {
                IPEndPoint end = null;
                do
                {
                    udp.Receive(ref end);
                } while (udp.Available > 0);
            }

            udp.BeginReceive(DiscoveryAction, udp);
            tcpListener.BeginAcceptTcpClient(DesktopConnection, tcpListener);
        }
    }
}
