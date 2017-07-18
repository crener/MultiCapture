using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Hub.Util;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class SavedProjectsTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new SavedProjectsResponse();
        }

        [Test]
        public void Response()
        {
            Deployer.Mock = true;
            Deployer.Start();

            byte[] value = response.GenerateResponse(ScannerCommands.CurrentProjects, null);
            string result = Encoding.ASCII.GetString(value);

            Assert.AreEqual(Deployer.ProjectManager.Json, result);
        }
    }
}
