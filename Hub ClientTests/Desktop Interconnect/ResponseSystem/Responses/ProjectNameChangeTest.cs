using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Hub.DesktopInterconnect.ResponseSystem.Responses;
using NUnit.Framework;

namespace Hub.ResponseSystem.Responses
{
    class ProjectNameChangeTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new ProjectNameChange();
        }

        [TestCase(null, null)]
        [TestCase("2", null)]
        [TestCase(null, "5")]
        [TestCase("e", "5")]
        [TestCase("e", "r")]
        [TestCase("1", "r")]
        public void IncorrectParameters(string id, string set)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (id != null) param.Add("id", id);
            if (set != null) param.Add("name", set);

            byte[] result = response.GenerateResponse(ScannerCommands.ProjectNameChange, param);

            string resultStr = Encoding.ASCII.GetString(result);
            Assert.IsTrue(resultStr.StartsWith(ResponseConstants.FailString));
        }
    }
}
