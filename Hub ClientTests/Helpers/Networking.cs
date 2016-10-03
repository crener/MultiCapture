using System;
using System.Net.Sockets;
using NUnit.Framework;
using System.Text;
using Hub.Helpers.Wrapper;
using SharedDeviceItems;

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
            catch (Exception e)
            {
                //threw an exception
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        /// <summary>
        /// Test that TrimExcessData correctly trims extra data from the end of the dataincluding the end of string message
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

            Assert.True(returnData.Length == data.Length);
            for (int i = 0; i < returnData.Length; i++)
            {
                if(returnData[i] != data[i]) Assert.Fail("Return value " + i + " didn;t match original data");
            }
        }

        [Test]
        public void Connected()
        {
            WSocket socket = new WSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Assert.IsFalse(socket.Connected);

            //todo find a way to test if a socket is connected without having to connect to an external device
        }
    }
}