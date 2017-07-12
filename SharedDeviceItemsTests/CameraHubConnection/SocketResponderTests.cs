using System;
using System.IO;
using System.Text;
using Hub.Helpers;
using NUnit.Framework;
using SharedDeviceItems;
using SharedDeviceItems.Networking.CameraHubConnection;

namespace SharedDeviceItemsTests.CameraHubConnection
{
    [TestFixture]
    class SocketResponderTests
    {
        [TestCase(12)]
        [TestCase(38)]
        [TestCase(101)]
        [TestCase(Constants.CameraBufferSize + 23)]
        [TestCase(Constants.CameraBufferSize)]
        [TestCase(Constants.CameraBufferSize * 4 + 8)]
        public void RecieveDataSimple(int dataSize)
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(dataSize, out package);
            mock.RecieveData = input;

            byte[] output = testclass.RecieveData();

            Assert.AreEqual(input.Length, mock.RecievePosition);
            Assert.AreEqual(package, output);
        }

        [Test]
        public void RecieveCommand()
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package = new CommandBuilder(CameraRequest.SendFullResImage)
                .AddParam(Constants.CameraCaptureImageName, "test")
                .Build();
            byte[] input = InterconnectHelper.FormatSendData(package);
            mock.RecieveData = input;

            byte[] output = testclass.RecieveData();

            Assert.AreEqual(input.Length, mock.RecievePosition);
            Assert.AreEqual(package, output);
        }

        [Test]
        public void DisconnectMidRecieve()
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(Constants.HubBufferSize * 2 - 80, out package);
            mock.RecieveData = input;
            mock.DisconnectMidTransmission = true;

            try
            {
                testclass.RecieveData();
                Assert.Fail("No exception was thrown");
            }
            catch (SocketDisconnectedException)
            {
                Assert.Pass("Correct exception was thrown");
            }

            Assert.Fail();
        }

        [Test]
        public void RecieveNotConnected()
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(1, out package);
            mock.RecieveData = input;
            mock.Connected = false;

            try
            {
                testclass.RecieveData();
                Assert.Fail("No exception was thrown");
            }
            catch (SocketNotConnectedException)
            {
                Assert.Pass("Correct exception was thrown");
            }
        }

        [Test]
        public void RecieveDisconnect()
        {
            MockSocket mock = new MockSocket();
            mock.Connected = true;
            mock.OverridePollFalse = true;
            SocketResponder testclass = new SocketResponder(mock);

            testclass.Disconnect();

            Assert.IsTrue(mock.Closed);
        }

        [Test]
        public void RecieveDisconnectException()
        {
            MockSocket mock = new MockSocket();
            mock.Connected = false;
            SocketResponder testclass = new SocketResponder(mock);

            try
            {
                testclass.Disconnect();
                Assert.Fail("exception should have been thrown");
            }
            catch(SocketNotConnectedException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void NoSize()
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            BuildRandomRequest(1, out package);
            mock.RecieveData = package;

            try
            {
                testclass.RecieveData();
                Assert.Fail("No exception was thrown");
            }
            catch (InvalidDataException)
            {
                Assert.Pass("Correct exception was thrown");
            }
        }

        [Test]
        public void SizeNotNumber()
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(1, out package);
            input[0] = 101; //letter: e
            mock.RecieveData = input;

            try
            {
                testclass.RecieveData();
                Assert.Fail("No exception was thrown");
            }
            catch (InvalidDataException)
            {
                Assert.Pass("Correct exception was thrown");
            }
        }

        [TestCase(12)]
        [TestCase(38)]
        [TestCase(101)]
        [TestCase(Constants.CameraBufferSize + 23)]
        [TestCase(Constants.CameraBufferSize)]
        [TestCase(Constants.CameraBufferSize * 4 + 8)]
        public void SendDataSimple(int dataSize)
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(dataSize, out package);
            mock.OverridePollFalse = true;

            testclass.SendResponse(package);

            Assert.AreEqual(input.Length, mock.SendData.Length);
            Assert.AreEqual(input, mock.SendData);
        }

        [Test]
        public void SendNotConnected()
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(1, out package);
            mock.RecieveData = input;
            mock.Connected = false;

            try
            {
                testclass.SendResponse(package);
                Assert.Fail("No exception was thrown");
            }
            catch (SocketNotConnectedException)
            {
                Assert.Pass("Correct exception was thrown");
            }
        }

        [Test]
        public void ClearSocket()
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(53, out package);
            mock.RecieveData = input;

            int cleared = testclass.ClearSocket();

            Assert.AreEqual(input.Length, cleared);
        }

        [TestCase(10)]
        [TestCase(Constants.CameraBufferSize + 12)]
        public void QueuedRequestException(int dataSize)
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(dataSize, out package);
            mock.RecieveData = input;

            byte[] output = testclass.RecieveData();

            Assert.AreEqual(package, output);
            mock.RecieveData = input;//reset sending position

            try
            {
                testclass.RecieveData();
                Assert.Fail("No exception was thrown");
            }
            catch (ResponseNeededException)
            {
                Assert.Pass("Correct exception was thrown");
            }
        }

        [TestCase(10)]
        [TestCase(Constants.CameraBufferSize + 12)]
        public void QueuedRequestSolveConnect(int dataSize)
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(dataSize, out package);
            mock.RecieveData = input;

            byte[] output = testclass.RecieveData();

            Assert.AreEqual(package, output);
            mock.RecieveData = input;//reset sending position

            testclass.Connect(mock);

            try
            {
                testclass.RecieveData();
            }
            catch (ResponseNeededException)
            {
                Assert.Fail("Response should not be needed");
            }
        }

        [TestCase(10)]
        [TestCase(Constants.CameraBufferSize + 12)]
        public void QueuedRequestSolveSend(int dataSize)
        {
            MockSocket mock = new MockSocket();
            SocketResponder testclass = new SocketResponder(mock);

            byte[] package;
            byte[] input = BuildRandomRequest(dataSize, out package);
            mock.RecieveData = input;

            byte[] output = testclass.RecieveData();

            Assert.AreEqual(package, output);
            mock.RecieveData = input;//reset sending position

            testclass.SendResponse(new byte[3]);

            try
            {
                testclass.RecieveData();
            }
            catch (ResponseNeededException)
            {
                Assert.Fail("Response should not be needed");
            }
        }

        public static byte[] BuildRandomRequest(int dataSize, out byte[] randomData)
        {
            int position = 0;
            byte[] input = new byte[dataSize + dataSize.ToString().Length + Constants.EndOfMessageBytes.Length];
            byte[] temp = Encoding.ASCII.GetBytes(dataSize.ToString());

            Array.Copy(temp, 0, input, position, temp.Length);
            position += temp.Length;

            temp = Constants.EndOfMessageBytes;

            Array.Copy(temp, 0, input, position, temp.Length);
            position += temp.Length;

            randomData = new byte[dataSize];
            new Random().NextBytes(randomData);

            Array.Copy(randomData, 0, input, position, randomData.Length);

            return input;
        }
    }
}
