using System;
using System.IO;
using NUnit.Framework;
using SharedDeviceItems;
using System.Collections.Generic;

namespace Hub.Util.Tests
{

    [TestFixture]
    class ProjectMapperTest
    {
        string saveLocation = Path.DirectorySeparatorChar + "scanimage" +
            Path.DirectorySeparatorChar + ProjectMapper.FileName;

        [SetUp]
        public void setup()
        {
            if (File.Exists(saveLocation)) File.Delete(saveLocation);
        }

        [Test]
        public void ImageCount()
        {
            ProjectMapper mapper = new ProjectMapper(saveLocation, new Random().Next());
            mapper.AddCamera(0, "test");

            mapper.AddImageSet(0, "nope");
            mapper.AddImage(0, "hub0.jpg", 0);
            mapper.AddImage(0, "zeroOne0.jpg", 0);
            mapper.AddImage(0, "zeroTwo0.jpg", 0);

            Assert.AreEqual(1, mapper.ImageSetCount);
            Assert.AreEqual(3, mapper.ImageCount(0));

            mapper.AddImageSet(1, "yes");
            mapper.AddImage(1, "hub1.jpg", 0);
            mapper.AddImage(1, "zeroOne1.jpg", 0);

            Assert.AreEqual(2, mapper.ImageSetCount);
            Assert.AreEqual(2, mapper.ImageCount(1));

            mapper.AddImageSet(2, "more");
            mapper.AddImage(2, "hub2.jpg", 0);

            Assert.AreEqual(3, mapper.ImageSetCount);
            Assert.AreEqual(1, mapper.ImageCount(2));

            mapper.AddImageSet(3, "test");

            Assert.AreEqual(4, mapper.ImageSetCount);
            Assert.AreEqual(0, mapper.ImageCount(3));

            mapper.AddImageSet(4, "other");
            mapper.AddImage(4, "hub2.jpg", 0);

            Assert.AreEqual(5, mapper.ImageSetCount);
            Assert.AreEqual(1, mapper.ImageCount(4));
        }

        [Test]
        public void SetException()
        {
            try
            {
                ProjectMapper mapper = new ProjectMapper(saveLocation, new Random().Next());
                mapper.AddCamera(0, "test");

                mapper.AddImageSet(0, "one");
                mapper.AddImageSet(0, "two");
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void SetDoesntExistTestException()
        {
            try
            {
                ProjectMapper mapper = new ProjectMapper(saveLocation, new Random().Next());
                mapper.AddCamera(0, "test");

                mapper.AddImage(0, "one", 0);
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void ExceptionTest()
        {
            try
            {
                ProjectMapper mapper = new ProjectMapper(saveLocation, new Random().Next());
                mapper.AddCamera(0, "test");

                mapper.AddImageSet(0, "set");
                mapper.AddImage(0, "one", 0);
                mapper.AddImage(0, "one", 0);
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void AddUnordered()
        {
            ProjectMapper mapper = new ProjectMapper(saveLocation, new Random().Next());
            mapper.AddCamera(0, "test");

            mapper.AddImageSet(2, "Zero");
            mapper.AddImageSet(3, "one");
            mapper.AddImageSet(1, "two");
            mapper.AddImageSet(4, "three");
            mapper.AddImageSet(0, "four");

            Assert.AreEqual(5, mapper.ImageSetCount);

            mapper.AddImage(0, "expected", 0);
            mapper.Sent(0, "expected");

            Assert.AreEqual(true, mapper.hasSent(0, "expected"));
            Assert.IsTrue(mapper.ImageSetIsDone(0));
        }

        [Test]
        public void SaveNoExceptionTest()
        {
            ProjectMapper mapper = new ProjectMapper(saveLocation, new Random().Next());
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(1, "london", 0);
            mapper.AddImage(1, "Tokio", 0);
            mapper.AddImage(0, "sleep", 0);
            mapper.AddImage(1, "Amsterdam", 0);

            Assert.AreEqual(2, mapper.ImageSetCount);
            Assert.AreEqual(2, mapper.ImageCount(0));
            Assert.AreEqual(3, mapper.ImageCount(1));

            mapper.Save();

            Assert.IsTrue(File.Exists(saveLocation));
        }

        [Test]
        public void ReadExceptionTest()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(1, "london", 0);
            mapper.AddImage(1, "Tokio", 0);
            mapper.AddImage(0, "sleep", 0);
            mapper.AddImage(1, "Amsterdam", 0);
            mapper.Save();

            ProjectMapper reader = new ProjectMapper(saveLocation, project - 12);

            Assert.AreEqual(project, reader.saveData.ProjectId);
            Assert.AreEqual(2, reader.ImageSetCount);
            Assert.AreEqual(2, reader.ImageCount(0));
            Assert.AreEqual(3, reader.ImageCount(1));
        }

        [Test]
        public void ReadSendTest()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(1, "london", 0);
            mapper.AddImage(1, "Tokio", 0);
            mapper.AddImage(0, "sleep", 0);
            mapper.AddImage(1, "Amsterdam", 0);

            mapper.Sent(0, "cryo");
            mapper.Sent(1, "Amsterdam");

            mapper.Save();

            ProjectMapper reader = new ProjectMapper(saveLocation, project);

            Assert.AreEqual(project, reader.saveData.ProjectId);
            Assert.AreEqual(true, reader.hasSent(0, "cryo"));
            Assert.AreEqual(true, reader.hasSent(1, "Amsterdam"));
            Assert.AreEqual(false, reader.hasSent(1, "london"));
        }

        [Test]
        public void ImageCountExceptionTest()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(1, "london", 0);
            mapper.AddImage(1, "Tokio", 0);
            mapper.AddImage(0, "sleep", 0);
            mapper.AddImage(1, "Amsterdam", 0);

            try
            {
                mapper.ImageCount(2);
                Assert.Fail("Image set 2 doesn't exist");
            }
            catch (Exception) { };

            Assert.AreEqual(2, mapper.ImageCount(0));
            Assert.AreEqual(3, mapper.ImageCount(1));
        }

        [Test]
        public void AddImageExceptionTest()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(1, "london", 0);
            mapper.AddImage(1, "Tokio", 0);
            mapper.AddImage(0, "sleep", 0);
            mapper.AddImage(1, "Amsterdam", 0);

            try
            {
                mapper.AddImage(2, "nope", 0);
                Assert.Fail("Image set 2 doesn't exist");
            }
            catch (Exception) { };

            try
            {
                mapper.AddImageSet(2, null);
                Assert.Fail("Image doesn't exist");
            }
            catch (Exception) { };
        }

        [Test]
        public void SentExceptionTest()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(1, "london", 0);
            mapper.AddImage(1, "Tokio", 0);
            mapper.AddImage(0, "sleep", 0);
            mapper.AddImage(1, "Amsterdam", 0);

            try
            {
                mapper.Sent(1, "nope");
                Assert.Fail("Image doesn't exist");
            }
            catch (Exception) { };

            try
            {
                mapper.hasSent(1, "nope");
                Assert.Fail("Image doesn't exist");
            }
            catch (Exception) { };

            try
            {
                mapper.SendTime(1, "nope");
                Assert.Fail("Image doesn't exist");
            }
            catch (Exception) { };

            try
            {
                mapper.hasSent(2);
                Assert.Fail("Image Set doesn't exist");
            }
            catch (Exception) { };

            Assert.IsFalse(mapper.hasSent(0, "cryo"));
            Assert.IsTrue(mapper.SendTime(0, "cryo") == 0);
            mapper.Sent(0, "cryo");
            Assert.IsTrue(mapper.SendTime(0, "cryo") > DateTime.Now.AddSeconds(-1).ToFileTimeUtc());
            Assert.IsTrue(mapper.hasSent(0, "cryo"));

            mapper.Sent(0, "cryo");
            mapper.Sent(1, "london");
            mapper.Sent(1, "Tokio");
            mapper.Sent(0, "sleep");
            mapper.Sent(1, "Amsterdam");

            Assert.IsTrue(mapper.hasSent(0));
            Assert.IsTrue(mapper.hasSent(1));
        }

        [Test]
        public void SetDoneTest()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(0, "sleep", 0);

            try
            {
                mapper.ImageSetIsDone(2);
                Assert.Fail("Image Set doesn't exist");
            }
            catch (Exception) { };

            try
            {
                mapper.ImageSetIsDone(1);
                Assert.Fail("Image Set should not have any images");
            }
            catch (Exception) { };

            Assert.AreEqual(false, mapper.ImageSetIsDone(0));

            mapper.Sent(0, "cryo");
            mapper.Sent(0, "sleep");

            Assert.AreEqual(true, mapper.ImageSetIsDone(0));
        }

        [Test]
        public void ImageAbsolutePathTest()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");

            mapper.AddImage(0, "cryo", 0);
            mapper.AddImage(1, "london", 0);
            mapper.AddImage(1, "Tokio", 0);
            mapper.AddImage(0, "sleep", 0);
            mapper.AddImage(1, "Amsterdam", 0);

            try
            {
                mapper.AbsoluteImagePath(2, "nope");
                Assert.Fail("Image set 2 doesn't exist");
            }
            catch (Exception) { };

            try
            {
                mapper.AbsoluteImagePath(1, "nope");
                Assert.Fail("Image doesn't exist");
            }
            catch (Exception) { };

            Assert.IsTrue(mapper.AbsoluteImagePath(0, "cryo").Length > 5);
        }

        [Test]
        public void JumbledOrder()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);
            mapper.AddCamera(0, "test");
            mapper.AddImageSet(2, "other");
            mapper.AddImageSet(1, "next");
            mapper.AddImageSet(0, "car");

            mapper.AddImage(2, "waves", 0);
            mapper.AddImage(2, "octo", 0);
            mapper.AddImage(2, "bike", 0);

            mapper.AddImage(0, "feet", 0);
            mapper.AddImage(0, "hands", 0);
            mapper.AddImage(0, "heads", 0);

            mapper.AddImage(1, "maps", 0);
            mapper.AddImage(1, "charts", 0);
            mapper.AddImage(1, "graphs", 0);

            mapper.Save();
            mapper = new ProjectMapper(saveLocation, 2333);

            Assert.AreNotEqual(2333, mapper.saveData.ProjectId);

            Assert.AreEqual(saveLocation + "\\next\\maps", mapper.AbsoluteImagePath(1, "maps"));
            Assert.AreEqual(saveLocation + "\\other\\bike", mapper.AbsoluteImagePath(2, "bike"));
            Assert.AreEqual(saveLocation + "\\car\\feet", mapper.AbsoluteImagePath(0, "feet"));
        }

        [Test]
        public void cameraIdSave()
        {
            int project = new Random().Next();
            ProjectMapperExtra mapper = new ProjectMapperExtra(saveLocation, project);

            mapper.AddImageSet(2, "twelve");
            mapper.AddImageSet(1, "seven");

            mapper.AddImage(1, "yes.jpg", 0);
            mapper.AddImage(1, "no.png", 0);

            mapper.AddCamera(0, "one");
            mapper.AddCamera(1, "left");
            mapper.AddCamera(2, "right");

            mapper.Save();
            mapper = new ProjectMapperExtra(saveLocation, 12);

            List<ProjectMapper.Camera> cams = mapper.getCameraList();

            Assert.AreEqual(3, cams.Count);

            Assert.AreEqual(0, cams[0].CameraId);
            Assert.AreEqual("one", cams[0].CameraName);

            Assert.AreEqual(1, cams[1].CameraId);
            Assert.AreEqual("left", cams[1].CameraName);

            Assert.AreEqual(2, cams[2].CameraId);
            Assert.AreEqual("right", cams[2].CameraName);
        }

        [Test]
        public void cameraAlreadyExists()
        {
            int project = new Random().Next();
            ProjectMapper mapper = new ProjectMapper(saveLocation, project);

            try
            {
                mapper.AddCamera(0, "yes");
                mapper.AddCamera(0, "yes");
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        private class ProjectMapperExtra : ProjectMapper
        {
            public ProjectMapperExtra(string project, int projectID) : base(project, projectID)
            {
            }

            public List<Camera> getCameraList()
            {
                return data.cameras;
            }
        }
    }
}
