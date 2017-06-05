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
    class LogTest
    {
        LogResponse response;

        [SetUp]
        public void setup()
        {
            response = new LogResponse();
        }

        [Test]
        public void Response()
        {
            
        }

        [Test]
        public void RegisterFull()
        {
            IResponse resp = DesktopThread.Responders[ScannerCommands.getRecentLogFile];

            Assert.NotNull(resp);
            Assert.AreEqual(response.GetType(), resp.GetType());
        }

        [Test]
        public void RegisterDiff()
        {
            IResponse resp = DesktopThread.Responders[ScannerCommands.getRecentLogDiff];

            Assert.NotNull(resp);
            Assert.AreEqual(response.GetType(), resp.GetType());
        }

        [Test]
        public void Reset()
        {
            //check that no exceptions are thrown
            response.Reset();

            byte[] returned = response.GenerateResponse(ScannerCommands.getRecentLogDiff, null);
            string value = Encoding.ASCII.GetString(returned);

            Assert.IsTrue(value.Contains(ResponseConstants.FailString));
        }
    }
}
