using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.DesktopInterconnect;
using Hub.Helpers;
using Hub.Util;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class NameTest
    {
        NameResponse response;

        [SetUp]
        public void setup()
        {
            response = new NameResponse();

            Deployer.mock = true;
            Deployer.Start();
        }

        [Test]
        public void ResponseFail()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            byte[] value = response.GenerateResponse(ScannerCommands.setName, parameters);
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
                byte[] value = response.GenerateResponse(ScannerCommands.setName, parameters);
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

        [Test]
        public void Register()
        {
            IResponse resp = DesktopThread.Responders[ScannerCommands.setName];

            Assert.NotNull(resp);
            Assert.AreEqual(response.GetType(), resp.GetType());
        }

        [Test]
        public void Reset()
        {
            //check that no exceptions are thrown
            response.Reset();
        }
    }
}
