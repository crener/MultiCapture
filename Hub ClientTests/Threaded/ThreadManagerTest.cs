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
    class ThreadManagerTest
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
            manager = new ThreadManager(new SaveLoad.Data());

            Assert.NotNull(manager.SavePath);
            Assert.NotNull(Deployer.CurrentProject);
            Assert.AreNotEqual(0, Deployer.CurrentProject.saveData.ProjectId);
        }


        /// <summary>
        /// Test broken can't get the manager to actually tell the camera thread to do work because it needs an
        /// active thread to make it work. 
        /// </summary>
        /// <param name="request"></param>
        //[TestCase(CameraRequest.Unknown)]
        //[TestCase(CameraRequest.SendFullResImage)]
        [TestCase(CameraRequest.Alive)]
        //[TestCase(CameraRequest.SendTestImage)]
        public void CaptureTest(CameraRequest request)
        {
            MockICameraThread mockCam = new MockICameraThread();
            SaveLoad.Data data = new SaveLoad.Data();
            data.Cameras = new CameraConfiguration[0];
            manager = new ThreadManager(data, new ICameraThread[] { mockCam });

            if (request == CameraRequest.Unknown) manager.CaptureImageSet();
            else manager.CaptureImageSet(request);

            if (request == CameraRequest.Unknown) Assert.AreEqual(CameraRequest.SendFullResImage, mockCam.Request);
            else Assert.AreEqual(request, mockCam.Request);

            if (CameraHelper.SavesImage(request)) Assert.AreEqual("1", mockCam.ImageSetName);
            Assert.AreEqual(1, Deployer.CurrentProject.ImageSetCount);
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

            manager = new ThreadManager(data);

            Assert.AreEqual(0, Deployer.CurrentProject.CameraCount);
        }
    }
}
