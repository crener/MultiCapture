using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using SharedDeviceItems;
using Hub.Util;

namespace Hub.Util.Tests
{

    [TestFixture]
    class ImageMapperTest
    {
        string saveLocation = Path.DirectorySeparatorChar + "scanimage" +
            Path.DirectorySeparatorChar + "testProject." + Constants.ProjectFileExtention;

        [SetUp]
        public void setup()
        {
            if (File.Exists(saveLocation)) File.Delete(saveLocation);
        }

        [Test]
        public void ImageCount()
        {
            ImageMapper mapper = new ImageMapper(saveLocation, new Random().Next());
            mapper.AddImageSet(0, "nope");
            mapper.AddImage(0, "hub0.jpg");
            mapper.AddImage(0, "zeroOne0.jpg");
            mapper.AddImage(0, "zeroTwo0.jpg");

            Assert.AreEqual(1, mapper.ImageSetCount);
            Assert.AreEqual(3, mapper.ImageCount(0));

            mapper.AddImageSet(1, "yes");
            mapper.AddImage(1, "hub1.jpg");
            mapper.AddImage(1, "zeroOne1.jpg");

            Assert.AreEqual(2, mapper.ImageSetCount);
            Assert.AreEqual(2, mapper.ImageCount(1));

            mapper.AddImageSet(2, "more");
            mapper.AddImage(2, "hub2.jpg");

            Assert.AreEqual(3, mapper.ImageSetCount);
            Assert.AreEqual(1, mapper.ImageCount(2));

            mapper.AddImageSet(3, "test");

            Assert.AreEqual(4, mapper.ImageSetCount);
            Assert.AreEqual(0, mapper.ImageCount(3));

            mapper.AddImageSet(4, "other");
            mapper.AddImage(4, "hub2.jpg");

            Assert.AreEqual(5, mapper.ImageSetCount);
            Assert.AreEqual(1, mapper.ImageCount(4));
        }

        [Test]
        public void ImageSetException()
        {
            try
            {
                ImageMapper mapper = new ImageMapper(saveLocation, new Random().Next());
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
        public void ImageSetNonExistTestException()
        {
            try
            {
                ImageMapper mapper = new ImageMapper(saveLocation, new Random().Next());
                mapper.AddImage(0, "one");
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void ImageException()
        {
            try
            {
                ImageMapper mapper = new ImageMapper(saveLocation, new Random().Next());
                mapper.AddImageSet(0, "set");
                mapper.AddImage(0, "one");
                mapper.AddImage(0, "one");
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
            ImageMapper mapper = new ImageMapper(saveLocation, new Random().Next());
            mapper.AddImageSet(2, "Zero");
            mapper.AddImageSet(3, "one");
            mapper.AddImageSet(1, "two");
            mapper.AddImageSet(4, "three");
            mapper.AddImageSet(0, "four");

            Assert.AreEqual(5, mapper.ImageSetCount);

            mapper.AddImage(0, "expected");
            mapper.Sent(0, "expected");

            Assert.AreEqual(true, mapper.hasSent(0, "expected"));
            Assert.IsTrue(mapper.ImageSetIsDone(0));
        }

        [Test]
        public void SaveNoExceptionTest()
        {
            ImageMapper mapper = new ImageMapper(saveLocation, new Random().Next());
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");
            mapper.AddImage(0, "cryo");
            mapper.AddImage(1, "london");
            mapper.AddImage(1, "Tokio");
            mapper.AddImage(0, "sleep");
            mapper.AddImage(1, "Amsterdam");

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
            ImageMapper mapper = new ImageMapper(saveLocation, project);
            mapper.AddImageSet(1, "place");
            mapper.AddImageSet(0, "other");
            mapper.AddImage(0, "cryo");
            mapper.AddImage(1, "london");
            mapper.AddImage(1, "Tokio");
            mapper.AddImage(0, "sleep");
            mapper.AddImage(1, "Amsterdam");
            mapper.Save();

            ImageMapper reader = new ImageMapper(saveLocation, project - 12);

            Assert.AreNotEqual(project, reader.ProjectID);
            Assert.AreEqual(2, reader.ImageSetCount);
            Assert.AreEqual(2, reader.ImageCount(0));
            Assert.AreEqual(3, reader.ImageCount(1));
        }
    }
}
