using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Hub.Helpers;
using Hub.Util;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class NameTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new NameResponse();

            Deployer.Mock = true;
            Deployer.Start();
        }

        [Test]
        public void ResponseFail()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            byte[] value = response.GenerateResponse(ScannerCommands.SetName, parameters);
            string convertValue = Encoding.ASCII.GetString(value);

            Assert.IsTrue(convertValue.Contains(ResponseConstants.FailString));
            Assert.IsTrue(convertValue.Contains("parameter missing"));
            Assert.IsTrue(convertValue.Contains("name"));
        }

        [TestCase("rogger")]
        [TestCase("camera")]
        public void ResponseSuccess(string name)
        {
            string before = Deployer.SysConfig.name;

            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("name", name);
                byte[] value = response.GenerateResponse(ScannerCommands.SetName, parameters);
                string convertValue = Encoding.ASCII.GetString(value);

                Assert.AreEqual(ResponseConstants.SuccessResponse, value);
                Assert.AreEqual(Deployer.SysConfig.name, name);
            }
            finally
            {
                //restore original name
                Deployer.SysConfig = new SaveLoad.Data()
                {
                    Cameras = Deployer.SysConfig.Cameras,
                    name = before
                };
            }
        }
    }
}
