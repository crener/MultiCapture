using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharedDeviceItems;
using Camera_Server;
using Camera_ServerTests.Mocks;
using Hub.Helpers.Wrapper;
using Hub.Networking;
using SharedDeviceItems.Interface;

namespace Camera_ServerTests
{
    [TestFixture]
    class RequestProcessTest
    {
        private MockSocket socket = new MockSocket();
        private MockCamera cam = new MockCamera();

        [SetUp]
        public void Setup()
        {
            socket.Available = 0;
            socket.Connected = true;

            cam.name = "test camera";
            cam.imageY = 1080;
            cam.imageX = 1920;
            cam.identifier = "test";
            cam.cameraData = new byte[] {100, 234, 20, 30};
            if(!string.IsNullOrEmpty(cam.lastPath) && cam.lastPath != "\\scanimage\\test.jpg") File.Delete(cam.lastPath);
            cam.lastPath = "";
            cam.directory = Path.DirectorySeparatorChar + "scanimage" + Path.DirectorySeparatorChar;
        }

        [Test]
        public void AllRequestsNoException()
        {
            RequestProcess processer = new CustomCameraRequestProcess(socket, cam);

            CameraRequest[] allRequests = (CameraRequest[]) Enum.GetValues(typeof(CameraRequest));
            foreach (int value in allRequests)
            {
                //Run a request for each possible camera request
                string request = value + Constants.EndOfMessage;
                processer.ProcessRequest(request);
            }

            //prevent setup deleteing the test image
            cam.lastPath = "";
        }

    }

    class CustomCameraRequestProcess : RequestProcess
    {
        public CustomCameraRequestProcess(ISocket client) : base(client) {}

        public CustomCameraRequestProcess(ISocket client, ICamera camera) : base(client)
        {
            base.camera = camera;
        }
    }
}
