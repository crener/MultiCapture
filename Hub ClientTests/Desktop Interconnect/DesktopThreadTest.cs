using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub.Util;
using Hub_ClientTests.Desktop_Interconnect;
using Hub_ClientTests.Networking;
using NUnit.Framework;

namespace Hub.DesktopInterconnect
{
    public class DesktopThreadTest
    {
        [TestCase]
        public void Startup()
        {
            MockDesktopThread thread = new MockDesktopThread();

            Assert.Null(MockDesktopThread.Connection);
            Assert.Null(MockDesktopThread.Udp);
            Assert.Null(MockDesktopThread.TcpListener);

            thread.Start();

            Assert.NotNull(MockDesktopThread.Udp);
            Assert.True(MockDesktopThread.Udp.Client.IsBound);
            Assert.NotNull(MockDesktopThread.TcpListener);
        }

        /*[TestCase]
        public void DiscoveryReply()
        {
            MockDesktopThread threadTest = new MockDesktopThread();
            MockUdpClient udp = new MockUdpClient();
            MockDesktopThread.udp = udp;
            SaveLoad.Conf = new SaveLoad.Data { name = "testCase" };

            DesktopThread.Instance.Start();

            Assert.NotNull(MockDesktopThread.connection);
            Assert.NotNull(MockDesktopThread.udp);
            Assert.True(MockDesktopThread.udp.Client.IsBound);
            Assert.NotNull(MockDesktopThread.tcpListener);

        }*/
    }
}
