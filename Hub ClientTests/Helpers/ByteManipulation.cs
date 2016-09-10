using NUnit.Framework;
using System.Text;
using Hub.Helpers;
using SharedDeviceItems;

namespace Hub.Helpers.Tests
{
    [TestFixture]
    public class ByteManipulationTests
    {
        [Test]
        public void SeperateDataTestCorrect()
        {
            string name = "This is a name";
            byte[] data = { 0, 23, 243, 231, 23, 234, 234, 23, 4, 4, 4, 0, 0, 234 };

            //create byte array for seperation
            byte[] byteName = Encoding.ASCII.GetBytes(name + Constants.MessageSeperator);
            byte[] testData = new byte[byteName.Length + data.Length];
            byteName.CopyTo(testData, 0);
            data.CopyTo(testData, byteName.Length);

            //return data
            string returnName;
            byte[] returnBytes;

            Assert.IsTrue(ByteManipulation.SeperateData(out returnName, testData, out returnBytes));
            Assert.IsTrue(returnName.Equals(name));
            Assert.IsTrue(data.Length == returnBytes.Length);

            for (int i = 0; i < data.Length; i++)
                Assert.IsTrue(data[i] == returnBytes[i]);
        }

        [Test]
        public void SeperateDataTestIncorrect()
        {
            string name = "This is a really long string that needs to be parsed one by one";
            byte[] data = { 0, 23, 243, 231 };

            //create byte array for seperation
            byte[] byteName = Encoding.ASCII.GetBytes(name + Constants.EndOfMessage);
            byte[] testData = new byte[byteName.Length + data.Length];
            byteName.CopyTo(testData, 0);
            data.CopyTo(testData, byteName.Length);

            //return data
            string returnName;
            byte[] returnBytes;

            Assert.IsFalse(ByteManipulation.SeperateData(out returnName, testData, out returnBytes));
            Assert.IsTrue(returnName == "");
            Assert.IsTrue(returnBytes.Length == 0);
        }

        /// <summary>
        /// Test ensures that the seperator will continue to function when most of the 
        /// seperation character are there but not all of them
        /// </summary>
        [Test]
        public void SeperateDataHalfSeperated()
        {
            string name = "String";
            byte[] data = { 12, 34, 211, 179 };

            //create byte array for seperation
            string almostSeperator = "";
            for (int i = 0; i < Constants.MessageSeperator.Length - 1; i++)
            {
                almostSeperator += Constants.MessageSeperator[i];
            }

            name += almostSeperator;

            byte[] byteName = Encoding.ASCII.GetBytes(name + Constants.MessageSeperator);
            byte[] testData = new byte[byteName.Length + data.Length];
            byteName.CopyTo(testData, 0);
            data.CopyTo(testData, byteName.Length);

            //return data
            string returnName;
            byte[] returnBytes;

            Assert.IsTrue(ByteManipulation.SeperateData(out returnName, testData, out returnBytes));
            Assert.IsTrue(returnName.Equals(name));
            Assert.IsTrue(data.Length == returnBytes.Length);

            for (int i = 0; i < data.Length; i++)
                Assert.IsTrue(data[i] == returnBytes[i]);
        }
    }
}