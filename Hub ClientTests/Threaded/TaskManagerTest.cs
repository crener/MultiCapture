using System;
using System.IO;
using Hub.Helpers;
using Hub.Helpers.Interface;
using Hub.Util;
using NUnit.Framework;
using SharedDeviceItems;

namespace Hub.Threaded
{
    [TestFixture]
    class TaskManagerTest
    {
        private ICameraManager manager;

        [TearDown]
        public void CleanUp()
        {
            //cleanup the test project file that was generated
            Directory.Delete(manager.SavePath, true);
            Console.WriteLine("Test project file removed");
            manager = null;
        }

        [Test]
        public void Constructor()
        {
            manager = new TaskManager(new SaveLoad.Data());

            Assert.NotNull(manager.SavePath);
            Assert.NotNull(Deployer.CurrentProject);
            Assert.AreNotEqual(0, Deployer.CurrentProject.saveData.ProjectId);
        }


        [TestCase(CameraRequest.Unknown)]
        [TestCase(CameraRequest.SendFullResImage)]
        [TestCase(CameraRequest.Alive)]
        [TestCase(CameraRequest.SendTestImage)]
        public void CaptureTest(CameraRequest request)
        {
            MockICameraTask mockCam = new MockICameraTask();
            SaveLoad.Data data = new SaveLoad.Data();
            manager = new TaskManager(data, new ICameraTask[] { mockCam });

            if (request == CameraRequest.Unknown) manager.CaptureImageSet();
            else manager.CaptureImageSet(request);

            if (request == CameraRequest.Unknown) Assert.AreEqual(CameraRequest.SendFullResImage, mockCam.lastRequest);
            else Assert.AreEqual(request, mockCam.lastRequest);

            if(CameraHelper.SavesImage(request)) Assert.AreEqual("1", mockCam.ImageSetName);
        }


        [Test]
        public void FailedCameraConnection()
        {
            SaveLoad.Data data = new SaveLoad.Data();
            data.Cameras = new[]
            {
                new CameraConfiguration {
                    Address = 12,
                    CamFileIdentity = "test",
                    Id = 0,
                    Port = 344}
            };

            manager = new TaskManager(data);

            Assert.AreEqual(0, Deployer.CurrentProject.CameraCount);
        }
    }
}
