using System;
using System.Net.Sockets;
using NUnit.Framework;
using System.Text;
using SharedDeviceItems;
using SharedDeviceItems.Networking;

namespace Hub.Helpers.Tests
{
    [TestFixture]
    public class NetworkingTests
    {
        /// <summary>
        /// Test that TrimExcessData throws an exception when there is no end of message in the data
        /// </summary>
        [Test]
        public void DataTrimTestException()
        {
            byte[] data = { 12, 145, 241, 17, 3, 211, 154, 172, 104, 238, 74, 25 };
            bool exceptionThrown = false;

            try
            {
                Networking.TrimExcessByteData(data);
            }
            catch (Exception)
            {
                //threw an exception
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        /// <summary>
        /// Test that TrimExcessData correctly trims extra data from the end of the data including the end of string message
        /// </summary>
        [Test]
        public void DataTrimTestAccurateData()
        {
            byte[] data = { 12, 145, 241, 17, 3, 211, 154, 172, 104, 238, 74, 25 };
            byte[] endOfMessage = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] extra = { 23, 23, 23, 23, 23, 23 };

            byte[] testData = new byte[data.Length + endOfMessage.Length + extra.Length];
            data.CopyTo(testData, 0);
            endOfMessage.CopyTo(testData, data.Length);
            extra.CopyTo(testData, data.Length + endOfMessage.Length);

            byte[] returnData = null;

            try
            {
                returnData = Networking.TrimExcessByteData(testData);
            }
            catch (Exception e)
            {
                //threw an exception
                Assert.Fail("An exception should not be thrown. " + e);
            }

            Assert.AreEqual( data.Length, returnData.Length);
            for (int i = 0; i < returnData.Length; i++)
            {
                if(returnData[i] != data[i]) Assert.Fail("Return value " + i + " didn't match original data");
            }
        }

        [Test]
        public void Connected()
        {
            SocketWrapper socketWrapper = new SocketWrapper(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Assert.IsFalse(socketWrapper.Connected);
        }
    }
}