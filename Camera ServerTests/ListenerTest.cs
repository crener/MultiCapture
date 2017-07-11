using System.Net;
using CameraServerTests.Mocks;
using Hub.Networking;
using NUnit.Framework;
using SharedDeviceItems.Networking.CameraHubConnection;

namespace CameraServer.Tests
{
    [TestFixture]
    public class ListenerTest
    {
        [Test]
        public void RespondToRequest()
        {
            TestListener testcase = new TestListener();
            MockRequestProcess mockProcess = new MockRequestProcess(new MockCamera());
            testcase.Process = mockProcess;
            MockIResponder responder = new MockIResponder();
            testcase.Responder = responder;

            //setup responses
            byte[] request = new byte[] { 0, 2, 3, 11, 12 };
            responder.RecieveBytes = request;
            byte[] response = new byte[] { 0, 222, 32, 66, 211 };
            mockProcess.RequestResponse = response;

            testcase.StartListening();

            Assert.AreEqual(request, mockProcess.IncomingRequest);
            Assert.AreEqual(response, responder.SendBytes);
        }

        private class TestListener : Listener
        {
            public RequestProcess Process { get; set; }
            public IResponder Responder { get; set; }

            public TestListener() { }

            public TestListener(ISocket socket)
            {
                if (socket != null) base.listener = socket;
            }

            public TestListener(ISocket socket, RequestProcess process)
            {
                if (socket != null) base.listener = socket;
                this.Process = process;
            }

            public bool Stop
            {
                get { return stop; }
                set { stop = value; }
            }

            protected override void SetupSocket(IPEndPoint localEndPoint)
            {
                listener = new MockSocket();
            }

            protected override RequestProcess NewProcessor()
            {
                return Process ?? new MockRequestProcess(new MockCamera());
            }

            protected override IResponder NewResponder()
            {
                return Responder ?? base.NewResponder();
            }
        }
    }
}
