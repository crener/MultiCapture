using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Hub.Util;
using Newtonsoft.Json;
using NUnit.Framework;
using SharedDeviceItems;

namespace Hub.ResponseSystem.Responses
{
    class LogTest : ResponseTests
    {
        private readonly string path = Constants.DefaultHubSaveLocation() + "testLog.txt";

        [SetUp]
        public override void Setup()
        {
            response = new LogResponse();
        }

        [Test]
        public void DiffContinue()
        {
            TestLogResponse resp = new TestLogResponse();
            resp.path = path;

            GenerateTextDocument(10, path);

            try
            {
                //get the full data
                byte[] result = resp.GenerateResponse(ScannerCommands.LogFile, null);
                List<string> full = JsonConvert.DeserializeObject<List<string>>(Encoding.ASCII.GetString(result));

                //check the full data is valid
                Assert.AreEqual(10, full.Count);
                for (int i = 0; i < full.Count; i++)
                    Assert.AreEqual(i.ToString(), full[i]);

                //add more to the file
                GenerateTextDocument(20, path);

                //get diff data
                result = resp.GenerateResponse(ScannerCommands.LogDiff, null);
                List<string> diff = JsonConvert.DeserializeObject<List<string>>(Encoding.ASCII.GetString(result));

                //check the diff data is valid
                Assert.AreEqual(10, diff.Count);
                for (int i = 0; i < diff.Count; i++)
                    Assert.AreEqual((10 + i).ToString(), diff[i]);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Test]
        public void DiffNoFile()
        {
            TestLogResponse resp = new TestLogResponse();
            resp.path = path;

            GenerateTextDocument(10, path);

            try
            {
                //get the full data
                byte[] result = resp.GenerateResponse(ScannerCommands.LogFile, null);
                List<string> full = JsonConvert.DeserializeObject<List<string>>(Encoding.ASCII.GetString(result));

                //check the full data is valid
                Assert.AreEqual(10, full.Count);
                for (int i = 0; i < full.Count; i++)
                    Assert.AreEqual(i.ToString(), full[i]);

                //remove the file
                File.Delete(path);

                //get diff data
                result = resp.GenerateResponse(ScannerCommands.LogDiff, null);
                string resultStr = Encoding.ASCII.GetString(result);

                Assert.IsTrue(resultStr.StartsWith(ResponseConstants.FailString));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Test]
        public void FullNoFile()
        {
            TestLogResponse resp = new TestLogResponse();
            resp.path = path;

            //get the full data
            byte[] result = resp.GenerateResponse(ScannerCommands.LogFile, null);
            string resultStr = Encoding.ASCII.GetString(result);

            Assert.IsTrue(resultStr.StartsWith(ResponseConstants.FailString));
        }

        [Test]
        public void ResetThenDiff()
        {
            Reset();

            byte[] returned = response.GenerateResponse(ScannerCommands.LogDiff, null);
            string value = Encoding.ASCII.GetString(returned);

            Assert.IsTrue(value.Contains(ResponseConstants.FailString));
        }

        private void GenerateTextDocument(int number, string filePath)
        {
            if (File.Exists(path)) File.Delete(path);

            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < number; i++)
                {
                    writer.WriteLine(i.ToString());
                }
            }
        }

        private class TestLogResponse : LogResponse
        {
            public string path;

            protected override string getPath()
            {
                return path ?? base.getPath();
            }
        }
    }
}
