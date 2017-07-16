using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class LogTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new LogResponse();
        }

        [Test]
        public void Response()
        {
            
        }

        [Test]
        public void ResetThenDiff()
        {
            Reset();

            byte[] returned = response.GenerateResponse(ScannerCommands.LogDiff, null);
            string value = Encoding.ASCII.GetString(returned);

            Assert.IsTrue(value.Contains(ResponseConstants.FailString));
        }
    }
}
