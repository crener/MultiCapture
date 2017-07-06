using Hub.DesktopInterconnect;
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
            byte[] value = response.GenerateResponse(ScannerCommands.ApiVersion, null);

            Assert.AreEqual(ResponseConstants.ApiResponse, value);
        }

        [Test]
        public void Register()
        {
            IResponse resp = DesktopThread.Responders[ScannerCommands.ApiVersion];

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
