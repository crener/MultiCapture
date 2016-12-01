using System.Net;
using System.Net.Sockets;
using System.Threading;
using Camera_Server;
using Hub.Helpers;
using Hub.Helpers.Wrapper;
using NUnit.Framework;
using SharedDeviceItems;
using SharedDeviceItems.Helpers;

namespace Hub.Threaded.Tests
{
    [TestFixture]
    public class CameraThreadTest
    {
        //[Test]
        public void CameraThreadConnectionTest()
        {
            Thread camServer = new Thread(new Listener().StartListening);
            camServer.Name = "CameraServer";
            camServer.Start();
            CameraConfiguration testConfig = new CameraConfiguration
            {
#pragma warning disable 618
                Address = NetworkHelpers.GrabIpv4(Dns.GetHostEntry(Dns.GetHostName())).Address,
#pragma warning restore 618
                Port = 11003,
                CamFileIdentity = "testCam",
                Id = -1
            };

            CameraSocket testSocket = new CameraSocket
            {
                DataSocket = new WSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                Config = testConfig
            };

            //start initialising the camera thread
            Assert.IsTrue(testSocket.Setup());
            CameraThread threadTask = new CameraThread(testSocket);
            Thread camThread = new Thread(threadTask.Start);
            camThread.Name = "CameraThread";
            camThread.Start();

            //Tell the camera to send a test image 
            threadTask.Request = CameraRequest.SendTestImage;
            Thread.Sleep(32);
            while (threadTask.Request != CameraRequest.Alive) Thread.Sleep(10);

            camThread.Join();
            camServer.Join();
        }
    }
}
