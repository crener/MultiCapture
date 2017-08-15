using System;
using System.IO;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.ResponseSystem;
using NUnit.Framework;
using SharedDeviceItems;

namespace Hub.ResponseSystem.Responses
{
    class CapacityTest : ResponseTests
    {
        [SetUp]
        public override void Setup()
        {
            response = new CapacityResponse();
        }

        [Test]
        public void BackUpSolution()
        {
            TestCapacity testCase = new TestCapacity();
            testCase.ready = true;

            DriveInfo[] drives = DriveInfo.GetDrives();
            string pathRoot = Path.GetPathRoot(Environment.SystemDirectory);
            DriveInfo actualDrive = null;

            //expected result
            foreach(DriveInfo drive in drives)
                if(pathRoot == drive.Name)
                    actualDrive = drive;

            //path which should fail initial try block
            testCase.path = "99" + Path.VolumeSeparatorChar;

            decimal freeMB = actualDrive.TotalFreeSpace / 1000000m;
            string formedString = freeMB.ToString("F3");

            byte[] value = testCase.GenerateResponse(ScannerCommands.Capacity, null);
            string result = Encoding.ASCII.GetString(value);

            Assert.IsTrue(result.StartsWith(ResponseConstants.SuccessString));
            Assert.IsTrue(result.EndsWith(formedString));
        }

        [Test]
        public void PrimarySolution()
        {
            TestCapacity testCase = new TestCapacity();
            testCase.ready = true;

            DriveInfo[] drives = DriveInfo.GetDrives();
            string pathRoot = Path.GetPathRoot(Environment.SystemDirectory);
            DriveInfo actualDrive = null;

            //expected result
            foreach (DriveInfo drive in drives)
                if (pathRoot == drive.Name)
                    actualDrive = drive;

            //path which should fail initial try block
            testCase.path = actualDrive.Name;

            decimal freeMB = actualDrive.TotalFreeSpace / 1000000m;
            string formedString = freeMB.ToString("F3");

            byte[] value = testCase.GenerateResponse(ScannerCommands.Capacity, null);
            string result = Encoding.ASCII.GetString(value);

            Assert.IsTrue(result.StartsWith(ResponseConstants.SuccessString));
            Assert.IsTrue(result.EndsWith(formedString));
        }

        [Test]
        public void NotReady()
        {
            byte[] value = response.GenerateResponse(ScannerCommands.Capacity, null);
            string result = Encoding.ASCII.GetString(value);

            Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
        }

        private class TestCapacity : CapacityResponse
        {
            public bool ready = false;
            public string path = null;

            protected override string getPath()
            {
                return path ?? Constants.DefaultHubSaveLocation();
            }

            protected override bool SystemReady()
            {
                return ready;
            }
        }
    }
}
