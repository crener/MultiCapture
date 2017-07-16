using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class ApiTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new ApiResponse();
        }

        [Test]
        public void Response()
        {
            byte[] value = response.GenerateResponse(ScannerCommands.ApiVersion, null);

            Assert.AreEqual(ResponseConstants.ApiResponse, value);
        }
    }
}
