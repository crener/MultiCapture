using System.Text;
using Hub.DesktopInterconnect;
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
            IResponse resp = DesktopThread.Responders[ScannerCommands.LogFile];

            Assert.NotNull(resp);
            Assert.AreEqual(response.GetType(), resp.GetType());
        }

        [Test]
        public void RegisterDiff()
        {
            IResponse resp = DesktopThread.Responders[ScannerCommands.LogDiff];

            Assert.NotNull(resp);
            Assert.AreEqual(response.GetType(), resp.GetType());
        }

        [Test]
        public void Reset()
        {
            //check that no exceptions are thrown
            response.Reset();

            byte[] returned = response.GenerateResponse(ScannerCommands.LogDiff, null);
            string value = Encoding.ASCII.GetString(returned);

            Assert.IsTrue(value.Contains(ResponseConstants.FailString));
        }
    }
}
