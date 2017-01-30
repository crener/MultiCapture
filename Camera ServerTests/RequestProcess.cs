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
            socket.receiveCount = 0;
            socket.receiveData = new byte[0];
            socket.sendData = new byte[0];

            cam.name = "test camera";
            cam.imageY = 1080;
            cam.imageX = 1920;
            cam.identifier = "test";
            cam.cameraData = new byte[] {100, 234, 20, 30};
            if(!string.IsNullOrEmpty(cam.lastPath) && !cam.lastPath.Contains("test.jpg")) File.Delete(cam.lastPath);
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

        [Test]
        public void UnrecognaisedRequest()
        {
            RequestProcess processer = new CustomCameraRequestProcess(socket, cam);
            processer.ProcessRequest(Encoding.ASCII.GetBytes("333" + Constants.EndOfMessage));

            byte[] returndata = socket.sendData;
            string returnString = Encoding.ASCII.GetString(returndata);

            Assert.IsTrue(returnString.Contains(Constants.FailString));

            processer.ProcessRequest("333" + Constants.EndOfMessage);
            returnString = Encoding.ASCII.GetString(socket.sendData);

            Assert.IsTrue(returnString.Contains(Constants.FailString));
        }

        //todo Camera data return test, response test for each request type, settings test to see if the settings are being applied to the camera

        //todo make a mixed test. Take the output from this and put it into the mock socket for the hub code so that it gets actual data in a test enviroment

        //todo make tests for the linsener code in the camera
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
