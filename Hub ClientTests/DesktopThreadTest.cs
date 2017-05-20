using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub_ClientTests.Networking;
using NUnit.Framework;

namespace Hub.DesktopInterconnect
{
    public class DesktopThreadTest
    {

        [TestCase]
        public void Startup()
        {
            MockDesktopThreadTest threadTest = new MockDesktopThreadTest();

            Assert.Null(MockDesktopThreadTest.connection);
            Assert.Null(MockDesktopThreadTest.udp);
            Assert.Null(MockDesktopThreadTest.tcpListener);

            threadTest.Startup();

            Assert.NotNull(MockDesktopThreadTest.connection);
            Assert.NotNull(MockDesktopThreadTest.udp);
            Assert.True(MockDesktopThreadTest.udp.Client.IsBound);
            Assert.NotNull(MockDesktopThreadTest.tcpListener);
        }

        [TestCase]
        public void DiscoveryReply()
        {
            MockDesktopThreadTest threadTest = new MockDesktopThreadTest();
            threadTest.Startup();

            Assert.NotNull(MockDesktopThreadTest.connection);
            Assert.NotNull(MockDesktopThreadTest.udp);
            Assert.True(MockDesktopThreadTest.udp.Client.IsBound);
            Assert.NotNull(MockDesktopThreadTest.tcpListener);

            MockSocket socket = new MockSocket();
            MockDesktopThreadTest.udp.Client = socket;
        }
    }
}
