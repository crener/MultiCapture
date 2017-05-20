using System;
using System.Text;
using Hub.Helpers;
using NUnit.Framework;
using Hub_ClientTests.Networking;
using SharedDeviceItems;

namespace Hub.Networking
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

            byte[] raw = new byte[qty];
            byte[] EOM = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] socketData = new byte[qty + EOM.Length];
            Array.Copy(raw, socketData, raw.Length);
            Array.Copy(EOM, 0, socketData, raw.Length, EOM.Length);

            socket.ReturnData = socketData;

            byte[] netData = net.MakeRequest(new byte[] { 22, 88, 45 });
            Assert.AreEqual(socket.ReturnData, netData);
        }



        [Test]
        public void EndSequenceInMiddleOfData()
        {
            socket.Connected = true;
            socket.FailCount = 1;
            socket.byteCount = 0;
            socket.maxSend = 0;

            byte[] part1 = new byte[400];
            byte[] part2 = new byte[400];
            byte[] EOM = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            int socketDataWritten = 0;
            byte[] socketData = new byte[part1.Length + part1.Length + (EOM.Length * 2)];
            Array.Copy(part1, socketData, part1.Length);
            socketDataWritten += part1.Length;
            Array.Copy(EOM, 0, socketData, socketDataWritten, EOM.Length);
            socketDataWritten += EOM.Length;
            Array.Copy(part2, 0, socketData, socketDataWritten, part2.Length);
            socketDataWritten += part2.Length;
            Array.Copy(EOM, 0, socketData, socketDataWritten, EOM.Length);

            socket.ReturnData = socketData;

            byte[] netData = net.MakeRequest(new byte[] { 22, 88, 45 });
            Assert.AreEqual(socket.ReturnData, netData);
        }
    }
}
