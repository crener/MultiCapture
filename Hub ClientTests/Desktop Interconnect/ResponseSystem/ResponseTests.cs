using Hub.ResponseSystem;
using NUnit.Framework;

namespace Hub.DesktopInterconnect.ResponseSystem
{
    [TestFixture]
    abstract class ResponseTests
    {
        protected IResponse response;

        public abstract void Setup();

        [Test]
        public void Register()
        {
            object[] responseTypes = GetType().GetCustomAttributes(typeof(ResponseTypeAttribute), false);

            foreach (ResponseTypeAttribute responseType in responseTypes)
            {
                IResponse resp = DesktopThread.Responders[responseType.Response];

                Assert.NotNull(resp);
                Assert.AreEqual(response.GetType(), resp.GetType());
            }
        }

        [Test]
        public void Reset()
        {
            //check that no exceptions are thrown
            response.Reset();
        }
    }
}
