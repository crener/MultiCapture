using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.DesktopInterconnect;
using Hub.Util;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class ApiTest
    {
        ApiResponse response;

        [SetUp]
        public void setup()
        {
            response = new ApiResponse();
        }

        [Test]
        public void Response()
        {
            byte[] value = response.GenerateResponse(ScannerCommands.getApiVersion, null);

            Assert.AreEqual(ResponseConstants.ApiResponse, value);
        }

        [Test]
        public void Register()
        {
            IResponse resp = DesktopThread.Responders[ScannerCommands.getApiVersion];

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
