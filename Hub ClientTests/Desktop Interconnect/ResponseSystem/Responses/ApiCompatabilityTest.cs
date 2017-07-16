using System;
using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

namespace Hub.ResponseSystem.Responses
{
    class ApiCompatabilityTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            DesktopThread.Responders.Clear();
            response = new ApiCompatability();
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(7)]
        public void Response(int count)
        {
            {
                int i = -1;
                foreach (ScannerCommands scannerCommand in Enum.GetValues(typeof(ScannerCommands)))
                {
                    i++;
                    if(i == 0 || scannerCommand.Equals(ScannerCommands.ApiCompatability)) continue;

                    DesktopThread.Responders.Add(scannerCommand, null);

                    if(i > count) break;
                }
            }

            byte[] value = response.GenerateResponse(ScannerCommands.ApiCompatability, null);

            Dictionary<int, string> sortedResponses =
                JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.ASCII.GetString(value));

            Assert.AreEqual(count + 1, sortedResponses.Count);
            foreach(KeyValuePair<int, string> pair in sortedResponses)
                Assert.IsTrue(DesktopThread.Responders.ContainsKey((ScannerCommands) pair.Key));
        }
    }
}
