using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using Hub.Util;
using NUnit.Framework;
using SharedDeviceItems;

namespace Hub.ResponseSystem.Responses
{
    class RemoveProjectTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new RemoveProjectResponse();
        }

        [Test]
        public void MissingParameter()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            byte[] value = response.GenerateResponse(ScannerCommands.RemoveProject, parameters);
            string result = Encoding.ASCII.GetString(value);

            Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
        }

        [Test]
        public void NoFile()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Random rand = new Random();
            string path = Constants.DefaultHubSaveLocation();
            int id;

            //generate an id that isn't used
            do
            {
                id = rand.Next();
            } while (Directory.Exists(path + id));
            path += id;

            //Directory.CreateDirectory(path);
            parameters.Add("id", id.ToString());

            try
            {
                byte[] value = response.GenerateResponse(ScannerCommands.RemoveProject, parameters);
                string result = Encoding.ASCII.GetString(value);

                Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
                Assert.IsFalse(Directory.Exists(path));
            }
            finally
            {
                if (Directory.Exists(path)) Directory.Delete(path);
            }
        }

        [Test]
        public void NotInProjectManager()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Random rand = new Random();
            string path = Constants.DefaultHubSaveLocation();
            int id;

            //generate an id that isn't used
            do
            {
                id = rand.Next();
            } while (Directory.Exists(path + id));
            path += id;

            Deployer.Mock = true;
            Deployer.Start();
            Directory.CreateDirectory(path);
            parameters.Add("id", id.ToString());

            try
            {
                byte[] value = response.GenerateResponse(ScannerCommands.RemoveProject, parameters);
                string result = Encoding.ASCII.GetString(value);

                Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
                Assert.IsFalse(Deployer.ProjectManager.ProjectExists(id));
            }
            finally
            {
                if (Directory.Exists(path)) Directory.Delete(path);
            }
        }

        [Test]
        public void InvalidId()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Random rand = new Random();
            string path = Constants.DefaultHubSaveLocation();
            int id;

            //generate an id that isn't used
            do
            {
                id = rand.Next();
            } while (Directory.Exists(path + id));
            parameters.Add("id", "R");

            byte[] value = response.GenerateResponse(ScannerCommands.RemoveProject, parameters);
            string result = Encoding.ASCII.GetString(value);

            Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
        }
    }
}
