using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Hub.ResponseSystem;
using Hub.Util;
using SharedDeviceItems.Networking;

[assembly: InternalsVisibleTo("Hub ClientTests")]
namespace Hub.DesktopInterconnect
{
    /// <summary>
    /// Responsible for handeling incoming connection and handing that resulting socket connection to the a DesktopConnection
    /// </summary>
    /// <seealso cref="DesktopConnection"/>
    public class DesktopThread
    {
        private static DesktopThread thread;

        public static DesktopThread Instance
        {
            get
            {
                if (thread == null) thread = new DesktopThread();
                return thread;
            }
            protected set { thread = value; }
        }

        private const int DiscoveryPort = 8470;
        private const int DiscoveryResponsePort = 8471;
        private const int DesktopConnectionPort = 8472;
        private static volatile bool connected = false;
        private static volatile bool started = false;

        protected static DesktopConnection connection;
        protected static IUdpClient udp;
        protected static TcpListener tcpListener;

        public static Dictionary<ScannerCommands, IResponse> Responders = new Dictionary<ScannerCommands, IResponse>();

        public DesktopThread()
        {
            //activate and itialiase all responders with the base responder as a sub class
            foreach (Type type in Assembly.GetAssembly(typeof(IResponse)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseResponse))))
            {
                Activator.CreateInstance(type);
            }
        }

        public void Start()
        {
            if (started) return;
            started = true;

            //Desktop device discovery (D3) - external method to find device on the network
            IPEndPoint disc = new IPEndPoint(IPAddress.Any, DiscoveryPort);
            udp = new UdpClientWrapper();
            Socket udpSocket = udp.Client;
            udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSocket.ExclusiveAddressUse = false;
            udpSocket.Bind(disc);
            udp.BeginReceive(DiscoveryAction, udp);

            //Desktop device connection - direct interaction with device (stops D3)
            IPEndPoint desk = new IPEndPoint(IPAddress.Any, DesktopConnectionPort);
            tcpListener = new TcpListener(desk);
            try
            {
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(DesktopConnection, tcpListener);
            }
#if DEBUG
            catch (SocketException soc)
#else
            catch (SocketException)
#endif
            {
                Console.WriteLine("Unable to start Desktop connection socket. External Connection will be unavaliable until restart");
#if DEBUG
                Console.WriteLine(soc);
#endif

                udpSocket.Close();
            }
        }


        protected void DiscoveryAction(IAsyncResult result)
        {
            if (connected) return;

            IUdpClient state = (IUdpClient)result.AsyncState;
            state.BeginReceive(DiscoveryAction, state);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);

            byte[] data = state.EndReceive(result, ref endpoint);
            if(data == null) return;

            Console.WriteLine("Discovery connection from: {0}, message: {1}", endpoint, Encoding.ASCII.GetString(data));

            byte[] response = Encoding.ASCII.GetBytes(Deployer.SysConfig.name);
            endpoint.Port = DiscoveryResponsePort;

            state.Send(response, response.Length, endpoint);
        }


        protected void DesktopConnection(IAsyncResult result)
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

        public void Disconnected()
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

            //reset responders
            foreach (IResponse responder in Responders.Values)
            {
                responder.Reset();
            }

            udp.BeginReceive(DiscoveryAction, udp);
            tcpListener.BeginAcceptTcpClient(DesktopConnection, tcpListener);
        }
    }
}
