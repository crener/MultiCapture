using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hub.DesktopInterconnect;

namespace Hub.DesktopInterconnect
{
    class MockDesktopThreadTest : DesktopThreadTest
    {
        public static volatile bool connected = false;
        public static volatile bool started = false;

        public static DesktopConnection connection;
        public static UdpClient udp;
        public static TcpListener tcpListener;


    }
}
