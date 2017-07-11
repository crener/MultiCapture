using System.Text;
using CameraServerTests.Mocks;
using Hub.Helpers;
using NUnit.Framework;
using SharedDeviceItems;

namespace CameraServer.Tests
{
    [TestFixture]
    class RequestProcessTest
    {
        [Test]
        public void Alive()
        {
            MockCamera cam = new MockCamera();
            RequestProcess test = new RequestProcess(cam);

            byte[] command = new CommandBuilder().Request(CameraRequest.Alive).Build();

            //test a response with byte data
            byte[] response = test.ProcessRequest(command);
            Assert.AreEqual(Constants.SuccessStringBytes, response);

            //test a response with string data
            response = test.ProcessRequest(Encoding.ASCII.GetString(command));
            Assert.AreEqual(Constants.SuccessStringBytes, response);
        }

        [Test]
        public void Error()
        {
            MockCamera cam = new MockCamera();
            RequestProcess test = new RequestProcess(cam);

            byte[] command = new CommandBuilder().Request(CameraRequest.Unknown).Build();

            //test a response with byte data
            byte[] response = test.ProcessRequest(command);
            Assert.AreEqual(Constants.FailStringBytes, response);

            //test a response with string data
            response = test.ProcessRequest(Encoding.ASCII.GetString(command));
            Assert.AreEqual(Constants.FailStringBytes, response);
        }

        [Test]
        public void CaptureError()
        {
            MockCamera cam = new MockCamera();
            cam.CameraData = new byte[] { 22, 22, 22, 44, 11, 88, 11, 223, 112 };
            RequestProcess test = new RequestProcess(cam);

            byte[] command = new CommandBuilder()
                .Request(CameraRequest.SendFullResImage)
                .Build();

            byte[] response = test.ProcessRequest(command);
            Assert.AreEqual(Constants.FailStringBytes, response);
        }

        [Test]
        public void Capture()
        {
            MockCamera cam = new MockCamera();
            cam.CameraData = new byte[] { 22, 22, 22, 44, 11, 88, 11, 223, 112 };
            RequestProcess test = new RequestProcess(cam);

            byte[] command = new CommandBuilder()
                .Request(CameraRequest.SendFullResImage)
                .AddParam(Constants.CameraCaptureImageName, "testImage")
                .Build();

            byte[] response = test.ProcessRequest(command);
            Assert.AreEqual(cam.CameraData, response);
        }

        [Test]
        public void SettingCameraName()
        {
            MockCamera cam = new MockCamera();
            cam.CameraData = new byte[] { 22, 22, 22, 44, 11, 88, 11, 223, 112 };
            RequestProcess test = new RequestProcess(cam);

            byte[] command = new CommandBuilder()
                .Request(CameraRequest.SetProporties)
                .AddParam(Constants.CameraSettingName, "TestCamera")
                .Build();

            byte[] response = test.ProcessRequest(command);
            Assert.AreEqual(Constants.SuccessStringBytes, response);
            Assert.AreEqual(cam.Name, "TestCamera");
        }
    }
}
