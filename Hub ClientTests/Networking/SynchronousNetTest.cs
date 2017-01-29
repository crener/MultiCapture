using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using Hub.Helpers;
using NUnit.Framework;
using Hub.Networking;
using SharedDeviceItems;

namespace Hub_ClientTests.Networking
{
    [TestFixture]
    public class SynchronousNetTest
    {
        private MockSocket socket;
        private SynchronousNet net;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            socket = new MockSocket();
            net = new SynchronousNet(socket);
        }

        [SetUp]
        public void Setup()
        {
            socket.recieveCount = 0;
            socket.FailCount = 0;
            socket.byteCount = 0;
        }

        [Test]
        public void DelayedDataReturn()
        {
            socket.Connected = true;
            socket.FailCount = 1;

            byte[] raw = new byte[] { 22, 32, 123, 23, 16, 44, 22, 88, 165, 231, 199, 199, 199, 124, 172, 144 };
            byte[] EOM = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] socketData = new byte[raw.Length + EOM.Length];
            Array.Copy(raw, socketData, raw.Length);
            Array.Copy(EOM, 0, socketData, raw.Length, EOM.Length);
            socket.ReturnData = socketData;

            byte[] netData = net.MakeRequest(new byte[] { 22, 88, 45 });
            Assert.AreEqual(socket.ReturnData, netData);

            socket.FailCount = 13;
            socket.recieveCount = 0;
            socket.byteCount = 0;
            netData = net.MakeRequest(new byte[] { 22, 88, 45 });
            Assert.AreEqual(socket.ReturnData, netData);
        }

        [TestCase(400000)]
        [TestCase(6874529)]
        [TestCase(875219)]
        public void LongData(int qty)
        {
            socket.Connected = true;
            socket.FailCount = 1;
            socket.byteCount = 0;
            socket.maxSend = 0;

            byte[] raw, socketData;
            do
            {
                raw = new byte[qty];
                new Random().NextBytes(raw);
                byte[] EOM = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
                socketData = new byte[raw.Length + EOM.Length];
                Array.Copy(raw, socketData, raw.Length);
                Array.Copy(EOM, 0, socketData, raw.Length, EOM.Length);
            } while (ByteManipulation.SearchEndOfMessageIndex(socketData, socketData.Length) != raw.Length);

            socket.ReturnData = socketData;

            byte[] netData = net.MakeRequest(new byte[] { 22, 88, 45 });
            Assert.AreEqual(socket.ReturnData, netData);
        }
    }
}
