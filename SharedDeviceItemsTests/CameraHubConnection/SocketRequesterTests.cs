using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharedDeviceItems;
using SharedDeviceItems.Networking.CameraHubConnection;

namespace SharedDeviceItemsTests.CameraHubConnection
{
    [TestFixture]
    class SocketRequesterTests
    {
        [TestCase(30, 2)]
        [TestCase(300, 20)]
        [TestCase(Constants.CameraBufferSize, 8)]
        [TestCase(Constants.CameraBufferSize * 3 + 67, 90)]
        [TestCase(300, Constants.CameraBufferSize * 2 + 5)]
        [TestCase(Constants.CameraBufferSize * 3 + 67, Constants.CameraBufferSize * 2 + 5)]
        [TestCase(Constants.CameraBufferSize, Constants.HubBufferSize)]
        public void RequestByte(int responseSize, int requestSize)
        {
            PairedSocket requesterSocket = new PairedSocket();
            PairedSocket responderSocket = new PairedSocket(requesterSocket);
            SocketRequester testclass = new SocketRequester(responderSocket);

            byte[] responseData;
            byte[] input = SocketResponderTests.BuildRandomRequest(responseSize, out responseData);
            byte[] requestData = new byte[requestSize];
            new Random().NextBytes(requestData);

            Task<List<byte[]>> responder = ResponseAction(responseData, requesterSocket, responderSocket);
            byte[] response = testclass.Request(requestData);
            responder.Wait();
            List<byte[]> recieved = responder.Result;

            Assert.AreEqual(responseData, response);

            Assert.AreEqual(input, responderSocket.RecieveData);
            Assert.AreEqual(requestData, recieved[1]);
        }


        [TestCase(CameraRequest.SendFullResImage, Constants.HubBufferSize)]
        [TestCase(CameraRequest.SendTestImage, 800)]
        [TestCase(CameraRequest.SetProporties, 60)]
        public void RequestCommand(CameraRequest request, int responseSize)
        {
            PairedSocket requesterSocket = new PairedSocket();
            PairedSocket responderSocket = new PairedSocket(requesterSocket);
            SocketRequester testclass = new SocketRequester(responderSocket);

            byte[] responseData;
            byte[] input = SocketResponderTests.BuildRandomRequest(responseSize, out responseData);
            byte[] requestData = new byte[responseSize];
            new Random().NextBytes(requestData);

            Task<List<byte[]>> responder = ResponseAction(responseData, requesterSocket, responderSocket);
            byte[] response = testclass.Request(request);
            responder.Wait();
            List<byte[]> recieved = responder.Result;

            Assert.AreEqual(responseData, response);

            Assert.AreEqual(input, responderSocket.RecieveData);
            Assert.AreEqual(Encoding.ASCII.GetBytes(((int)request).ToString()), recieved[1]);
        }

        [Test]
        public void LowInitialRecieveSize()
        {
            PairedSocket requesterSocket = new PairedSocket();
            PairedSocket responderSocket = new PairedSocket(requesterSocket);
            responderSocket.SubDivideRecieveData = Constants.HubBufferSize / 3;
            SocketRequester testclass = new SocketRequester(responderSocket);

            byte[] responseData;
            byte[] input = SocketResponderTests.BuildRandomRequest(Constants.HubBufferSize - 70, out responseData);
            byte[] requestData = new byte[Constants.HubBufferSize - 70];
            new Random().NextBytes(requestData);

            Task<List<byte[]>> responder = ResponseAction(responseData, requesterSocket, responderSocket);
            byte[] response = testclass.Request(CameraRequest.Alive);
            responder.Wait();
            List<byte[]> recieved = responder.Result;

            Assert.AreEqual(responseData, response);

            Assert.AreEqual(input, responderSocket.RecieveData);
        }

        async Task<List<byte[]>> ResponseAction(byte[] data, PairedSocket requesterSocket, PairedSocket responderSocket)
        {
            SocketResponder responder = new SocketResponder(requesterSocket);
            while (requesterSocket.RecieveData == null)
                await Task.Delay(5);

            List<byte[]> request = new List<byte[]>();
            request.Add(requesterSocket.RecieveData);//direct message from requester
            request.Add(responder.RecieveData());//processed response from requester

            responder.SendResponse(data);

            return request;
        }
    }
}
