using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SharedDeviceItems;
using Camera_Server;
using Camera_ServerTests.Mocks;
using CommandBuilder = Hub.Helpers.CommandBuilder;

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
            socket.ReceiveCount = 0;
            socket.RecieveQueryCount = 0;
            socket.ReceiveData = new byte[0];
            socket.SendData = new byte[0];

            cam.name = "camera";
            cam.imageY = 1080;
            cam.imageX = 1920;
            cam.identifier = "test";
            cam.cameraData = new byte[] { 100, 234, 20, 30 };
            if (!string.IsNullOrEmpty(cam.lastPath) && !cam.lastPath.Contains("test")) File.Delete(cam.lastPath);
            cam.lastPath = "";
            cam.directory = Path.DirectorySeparatorChar + "scanimage" + Path.DirectorySeparatorChar;
        }

        [Test]
        public void AllRequestsNoException()
        {
            RequestProcess processer = new MockRequestProcess(socket, cam);

            CameraRequest[] allRequests = (CameraRequest[])Enum.GetValues(typeof(CameraRequest));
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
            byte[] expectedResponse = Encoding.ASCII.GetBytes(Constants.FailString + Constants.EndOfMessage);
            RequestProcess processer = new MockRequestProcess(socket, cam);
            processer.ProcessRequest(Encoding.ASCII.GetBytes("333" + Constants.EndOfMessage));

            Assert.AreEqual(expectedResponse, socket.SendData);

            socket.SendData = new byte[] { };
            processer.ProcessRequest("333" + Constants.EndOfMessage);

            Assert.AreEqual(expectedResponse, socket.SendData);
        }

        [Test]
        public void AliveRequest()
        {
            byte[] expectedResponse = Encoding.ASCII.GetBytes(Constants.SuccessString + Constants.EndOfMessage);
            RequestProcess processer = new MockRequestProcess(socket, cam);
            byte[] request = new CommandBuilder().Request(CameraRequest.Alive).Build();

            processer.ProcessRequest(request);

            Assert.AreEqual(expectedResponse, socket.SendData);

            socket.SendData = new byte[] { };
            processer.ProcessRequest(Encoding.ASCII.GetString(request));

            Assert.AreEqual(expectedResponse, socket.SendData);
        }

        [Test]
        public void TestRequest()
        {
            RequestProcess processer = new MockRequestProcess(socket, cam);
            byte[] request = new CommandBuilder().Request(CameraRequest.SendTestImage).Build();

            byte[] name = Encoding.ASCII.GetBytes("test.jpg" + Constants.MessageSeperator),
                raw = File.ReadAllBytes(Constants.DefualtHubSaveLocation() + "test.jpg"),
                end = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] imageData = new byte[name.Length + raw.Length + end.Length];

            name.CopyTo(imageData, 0);
            raw.CopyTo(imageData, name.Length);
            end.CopyTo(imageData, name.Length + raw.Length);

            processer.ProcessRequest(request);

            Assert.AreEqual(imageData, socket.SendData);

            socket.SendData = new byte[] { };
            processer.ProcessRequest(Encoding.ASCII.GetString(request));

            Assert.AreEqual(imageData, socket.SendData);
        }

        [Test]
        public void FullRequest()
        {
            cam.cameraData = new byte[] { 39, 39, 39, 52, 78, 57, 112, 118, 189, 245, 230, 163 };
            cam.name = "full";
            string nameData = "byte";

            RequestProcess processer = new MockRequestProcess(socket, cam);
            byte[] request = new CommandBuilder().Request(CameraRequest.SendFullResImage).AddParam("id", nameData).Build();

            byte[] name = Encoding.ASCII.GetBytes(cam.name + nameData + ".jpg" + Constants.MessageSeperator),
                end = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] imageData = new byte[name.Length + cam.cameraData.Length + end.Length];

            name.CopyTo(imageData, 0);
            cam.cameraData.CopyTo(imageData, name.Length);
            end.CopyTo(imageData, name.Length + cam.cameraData.Length);

            processer.ProcessRequest(request);
            string converted = Encoding.ASCII.GetString(socket.SendData);

            Assert.AreEqual(imageData, socket.SendData);
            Assert.AreEqual(cam.name + nameData + ".jpg", converted.Substring(0, converted.IndexOf(Constants.MessageSeperator)));

            socket.SendData = new byte[] { };
            nameData = "string";
            name = Encoding.ASCII.GetBytes(cam.name + nameData + ".jpg" + Constants.MessageSeperator);
            imageData = new byte[name.Length + cam.cameraData.Length + end.Length];
            request = new CommandBuilder().Request(CameraRequest.SendFullResImage).AddParam("id", nameData).Build();

            name.CopyTo(imageData, 0);
            cam.cameraData.CopyTo(imageData, name.Length);
            end.CopyTo(imageData, name.Length + cam.cameraData.Length);

            processer.ProcessRequest(Encoding.ASCII.GetString(request));
            converted = Encoding.ASCII.GetString(socket.SendData);

            Assert.AreEqual(imageData, socket.SendData);
            Assert.AreEqual(cam.name + nameData + ".jpg", converted.Substring(0, converted.IndexOf(Constants.MessageSeperator)));
        }

        [Test]
        public void SettingName()
        {
            MockRequestProcess processer = new MockRequestProcess(socket, cam);
            byte[] request = new CommandBuilder().Request(CameraRequest.SetProporties).AddParam("name", "yeast").Build();
            processer.ProcessRequest(request);

            Assert.AreEqual("yeast", cam.name);

            processer.SetImageName("nope");
            processer.ProcessRequest(Encoding.ASCII.GetString(request));

            Assert.AreEqual("yeast", cam.name);
        }

        [Test]
        public void SettingId()
        {
            MockRequestProcess processer = new MockRequestProcess(socket, cam);
            byte[] request = new CommandBuilder().Request(CameraRequest.SetProporties).AddParam("id", "23").Build();
            processer.ProcessRequest(request);

            Assert.AreEqual("23", processer.GetImageName());

            processer.SetImageName("nope");
            processer.ProcessRequest(Encoding.ASCII.GetString(request));

            Assert.AreEqual("23", processer.GetImageName());
        }

        [Test]
        public void SettingNameAndId()
        {
            MockRequestProcess processer = new MockRequestProcess(socket, cam);
            byte[] request = new CommandBuilder().Request(CameraRequest.SetProporties).AddParam("name", "yeast").AddParam("id", "23").Build();
            processer.ProcessRequest(request);

            Assert.AreEqual("yeast", cam.name);
            Assert.AreEqual("23", processer.GetImageName());

            cam.name = "nope";
            processer.SetImageName("nope");
            processer.ProcessRequest(Encoding.ASCII.GetString(request));

            Assert.AreEqual("yeast", cam.name);
            Assert.AreEqual("23", processer.GetImageName());
        }

        //todo make a mixed test. Take the output from this and put it into the mock socket for the hub code so that it gets actual data in a test enviroment

        //todo make tests for the linsener code in the camera
    }
}
