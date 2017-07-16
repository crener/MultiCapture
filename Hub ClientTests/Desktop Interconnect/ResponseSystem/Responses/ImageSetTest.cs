using System.Collections.Generic;
using System.IO;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Hub.Util;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    [TestFixture]
    class ImageSetTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new ImageSetReponse();
        }

        [TestCase(null, null)]
        [TestCase("2", null)]
        [TestCase(null, "5")]
        [TestCase("e", "5")]
        [TestCase("e", "r")]
        [TestCase("1", "r")]
        public void IncorrectParameters(string id, string set)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (id != null) param.Add("id", id);
            if (set != null) param.Add("set", set);

            List<ScannerCommands> tests = new List<ScannerCommands>(new []{ScannerCommands.ImageSetMetaData, ScannerCommands.ImageSetImageData});

            foreach(ScannerCommands command in tests)
            {
                byte[] result = response.GenerateResponse(command, param);

                string resultStr = Encoding.ASCII.GetString(result);
                Assert.IsTrue(resultStr.StartsWith(ResponseConstants.FailString));
            }
        }

        [Test]
        public void NoValidProject()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", int.MaxValue.ToString());
            param.Add("set", int.MaxValue.ToString());

            Deployer.Mock = true;
            Deployer.Start();

            byte[] result = response.GenerateResponse(ScannerCommands.ImageSetMetaData, param);

            string resultStr = Encoding.ASCII.GetString(result);
            Assert.IsTrue(resultStr.StartsWith(ResponseConstants.FailString));
        }

        [Test]
        public void ImageSetResponse()
        {
            TestImageSet testResponse = new TestImageSet();

            ProjectMapper project = new ProjectMapper("23", 23);
            project.AddImageSet(2, "set-2");
            project.AddImage(2, "fake image.jpg", 0);

            testResponse.example = project;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", project.ProjectId.ToString());
            param.Add("set", "2");

            Deployer.Mock = true;
            Deployer.Start();

            byte[] result = testResponse.GenerateResponse(ScannerCommands.ImageSetMetaData, param);

            string resultStr = Encoding.ASCII.GetString(result);
            Assert.IsTrue(resultStr.Contains("fake image.jpg"));
            Assert.IsTrue(resultStr.Contains("2"));
        }

        [Test]
        public void ImageDataMissingResponse()
        {
            TestImageSet testResponse = new TestImageSet();

            ProjectMapper project = new ProjectMapper("23", 23);
            project.AddImageSet(2, "set-2");
            project.AddImage(2, "test.jpg", 0);

            testResponse.example = project;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", project.ProjectId.ToString());
            param.Add("set", "2");
            param.Add("image", "0");

            Deployer.Mock = true;
            Deployer.Start();

            byte[] result = testResponse.GenerateResponse(ScannerCommands.ImageSetImageData, param);

            string resultStr = Encoding.ASCII.GetString(result);
            Assert.IsTrue(resultStr.StartsWith(ResponseConstants.FailString));
        }

        [TestCase(null)]
        [TestCase("nope")]
        public void ImageDataIncorrect(string imageParameter)
        {
            TestImageSet testResponse = new TestImageSet();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", "90000");
            param.Add("set", "2");
            if(imageParameter == null) param.Add("image", imageParameter);

            Deployer.Mock = true;
            Deployer.Start();

            byte[] result = testResponse.GenerateResponse(ScannerCommands.ImageSetImageData, param);

            string resultStr = Encoding.ASCII.GetString(result);
            Assert.IsTrue(resultStr.StartsWith(ResponseConstants.FailString));
        }

        private class TestImageSet : ImageSetReponse
        {
            public ProjectMapper example;

            protected override bool FindProject(int projectId)
            {
                 if(example != null) projectCache.Add(projectId, example);
                return true;
            }
        }
    }
}
