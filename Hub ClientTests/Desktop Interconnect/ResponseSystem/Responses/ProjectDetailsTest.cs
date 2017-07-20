using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Hub.DesktopInterconnect.ResponseSystem.Responses;
using Hub.Util;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class ProjectDetailsTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new ProjectDetails();
        }

        [TestCase(null)]
        [TestCase("E")]
        [TestCase("33")]
        public void ParameterResponse(string val)
        {
            Dictionary<string, string> parameters = new Dictionary<string,string>();
            if(val != null) parameters.Add("id", val);

            Deployer.Mock = true;
            Deployer.Start();

            byte[] value = response.GenerateResponse(ScannerCommands.ProjectDetails, parameters);
            string result = Encoding.ASCII.GetString(value);

            Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
        }

        [Test]
        public void ValidRequest()
        {
            ProjectMapper testproject = new ProjectMapper("", 45);
            ProjectCache.ForceAddProjectMapper(45, testproject);

            Dictionary<string, string> parameters = new Dictionary<string,string>();
            parameters.Add("id", "45");

            byte[] value = response.GenerateResponse(ScannerCommands.ProjectDetails, parameters);
            string result = Encoding.ASCII.GetString(value);

            Assert.IsFalse(result.StartsWith(ResponseConstants.FailString));
        }
    }
}
