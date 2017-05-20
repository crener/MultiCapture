using System.Net.Sockets;
using SharedDeviceItems.Networking;

namespace Hub.DesktopInterconnect
{
    public class MockDesktopThread : DesktopThread
    {
        public static volatile bool connected = false;
        public static volatile bool started = false;

        public static DesktopConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }
        public static IUdpClient Udp
        {
            get { return udp; }
            set { udp = value; }
        }
        public static TcpListener TcpListener
        {
            get { return tcpListener; }
            set { tcpListener = value; }
        }

        public MockDesktopThread()
        {
            DesktopThread.Instance = this;
        }
    }
}
