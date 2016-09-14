using System;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using Hub.Helpers.Wrapper;
using Hub.Networking;
using Hub.SaveLoad;
using NUnit.Framework.Internal;
using SharedDeviceItems;
#pragma warning disable 618

namespace Hub.Helpers.Tests
{
    [TestFixture]
    public class DataClassTests
    {
        [Test]
        public void GetterSetterTest()
        {
            CameraSocket testCameraSocket = new CameraSocket();

            #region socket

            Assert.IsNull(testCameraSocket.DataSocket);

            WSocket testSocket = new WSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            testCameraSocket.DataSocket = testSocket;

            Assert.IsTrue(testCameraSocket.DataSocket != null);
            Assert.IsTrue(testCameraSocket.DataSocket.Equals(testSocket));

            ISocket testSocketget = testCameraSocket.DataSocket;
            Assert.IsTrue(testSocket.Equals(testSocketget));

            #endregion

            #region config

            CameraConfiguration camConfigTest = new CameraConfiguration
            {
                Id = 233,
                CamFileIdentity = "superTest string"
            };

            Assert.IsNull(testCameraSocket.Config);

            testCameraSocket.Config = camConfigTest;

            Assert.IsTrue(testCameraSocket.Config.Id == 233);
            Assert.IsTrue(testCameraSocket.Config.CamFileIdentity == "superTest string");

            #endregion
        }

        [Test]
        public void SetupTestException()
        {
            CameraSocket testSocket = new CameraSocket
            {
                DataSocket = new WSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                Config = new CameraConfiguration()
            };

            Exception ex = Assert.Throws<InvalidOperationException>(() => testSocket.Setup());
            Assert.That(ex.Message, Is.EqualTo("Configuration address not configured"));

            IPAddress address = Networking.GrabIpv4();
            testSocket.Config.Address = address.Address;
            ex = Assert.Throws<InvalidOperationException>(() => testSocket.Setup());
            Assert.That(ex.Message, Is.EqualTo("Configuration port not configured"));

            testSocket.Config.Port = 700;
            Assert.IsFalse(testSocket.Setup());

        }
    }
}