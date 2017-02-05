using System;
using System.Threading;
using Camera_Server;
using Camera_ServerTests.Mocks;
using Hub.Helpers;
using Hub.Networking;
using NUnit.Framework;
using SharedDeviceItems;

namespace Camera_ServerTests
{
    [TestFixture]
    public class ListenerTest
    {
        private Thread thread;
        private TestListener listen;
        private MockSocket socket;
        private MockRequestProcess process;

        [TearDown]
        public void Shutdown()
        {
            //make sure the thread is dead
            listen.Stop = true;
            thread.Interrupt();
            socket.Available = 0;
            thread.Join();
        }

        [SetUp]
        public void Setup()
        {
            socket = new MockSocket();
        }

        //causes test explorer to lock up so leaving it for now
        [Test]
        public void RecieveDataTest()
        {
            byte[] data = new byte[Constants.CameraBufferSize];
            byte[] request = new CommandBuilder().Request(CameraRequest.Alive).Build();
            request.CopyTo(data, 0);

            socket.Connected = true;
            socket.ReceiveData = data;
            socket.SlowDown = true;

            try
            {
                PrepairListener();

                //wait for the thread to call the socket at least once
                while(socket.RecieveQueryCount <= 0) Thread.Sleep(2);
                Thread.Sleep(10);
                listen.Stop = true;

                Assert.IsTrue(socket.ReceiveCount > 0);
                Assert.AreEqual(data, process.RequestProcessData);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                Shutdown();
            }
        }

        private void PrepairListener()
        {
            socket.RecieveQueryCount = 0;
            socket.SlowDown = false;
            process = new MockRequestProcess(socket, false);
            listen = new TestListener(socket, process);

            thread = new Thread(listen.StartListening);
            thread.Name = "Testing Thread";
            thread.Start();

            //allow the test thread to sleep so that the test thread is ready
            Thread.Sleep(10);
        }

        private class TestListener : Listener
        {
            private RequestProcess process;

            public TestListener() { }

            public TestListener(ISocket socket)
            {
                if (socket != null) base.listener = socket;
            }

            public TestListener(ISocket socket, RequestProcess process)
            {
                if (socket != null) base.listener = socket;
                this.process = process;
            }

            public bool Stop
            {
                get { return base.stop; }
                set { base.stop = value; }
            }

            protected override RequestProcess NewProcessor(ISocket handler)
            {
                if(process != null) return process;
                return new MockRequestProcess(handler);
            }
        }
    }
}
