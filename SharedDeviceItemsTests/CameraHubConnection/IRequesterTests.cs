using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Networking;
using NUnit.Framework;
using SharedDeviceItems;
using SharedDeviceItems.Networking.CameraHubConnection;

namespace SharedDeviceItemsTests.CameraHubConnection
{
    [TestFixture(new object[] {typeof(SocketRequester), typeof(SocketResponder)})]
    [TestFixture(new object[] {typeof(ChunkRequester), typeof(ChunkResponder)})]
    class RequesterTests<T, TR> where T : IRequester where TR : IResponder
    {
        public IRequester CreateRequester(ISocket socket)
        {
            if (typeof(T) == typeof(ChunkRequester))
                return new ChunkRequester(socket);

            //if (typeof(T) == typeof(SocketRequester))
            return new SocketRequester(socket);
        }
        public IResponder CreateResponder(ISocket socket)
        {
            if (typeof(TR) == typeof(ChunkResponder))
                return new ChunkResponder(socket);

            //if (typeof(R) == typeof(SocketResponder))
            return new SocketResponder(socket);
        }

        /// <summary>
        /// Simple test for ensuring that data comes out in the correct format
        /// </summary>
        /// <param name="responseSize">size of the data that is sent back as a response</param>
        /// <param name="requestSize">size of the incomming request</param>
        [TestCase(30, 2)]
        [TestCase(300, 20)]
        [TestCase(Constants.CameraBufferSize, 8)]
        [TestCase(Constants.CameraBufferSize * 3 + 67, 90)]
        [TestCase(300, Constants.CameraBufferSize * 2 + 5)]
        [TestCase(Constants.HubBufferSize * 3 + 67, Constants.CameraBufferSize * 2 + 5)]
        [TestCase(Constants.HubBufferSize, Constants.CameraBufferSize)]
        public void RequestByte(int responseSize, int requestSize)
        {
            PairedSocket requesterSocket = new PairedSocket();
            PairedSocket responderSocket = new PairedSocket(requesterSocket);
            IRequester testclass = CreateRequester(responderSocket);

            byte[] responseData;
            byte[] input = SocketResponderTests.BuildRandomRequest(responseSize, out responseData);
            byte[] requestData = new byte[requestSize];
            new Random().NextBytes(requestData);

            Task<List<byte[]>> responder = ResponseAction(responseData, requesterSocket, responderSocket);
            byte[] response = testclass.Request(requestData);
            responder.Wait();

            //Direct IRequester result
            Assert.AreEqual(responseData, response);

            //IResponder result
            //Assert.AreEqual(input, responderSocket.RecieveData);
        }

        /// <summary>
        /// Test simple sending with Camera Commands
        /// </summary>
        /// <param name="request">Camera request</param>
        /// <param name="responseSize">Length of the response data</param>
        [TestCase(CameraRequest.SendFullResImage, Constants.HubBufferSize)]
        [TestCase(CameraRequest.SendTestImage, 800)]
        [TestCase(CameraRequest.SetProporties, 60)]
        public void RequestCommand(CameraRequest request, int responseSize)
        {
            PairedSocket requesterSocket = new PairedSocket();
            PairedSocket responderSocket = new PairedSocket(requesterSocket);
            IRequester testclass = CreateRequester(responderSocket);

            byte[] responseData;
            byte[] input = SocketResponderTests.BuildRandomRequest(responseSize, out responseData);
            byte[] requestData = new byte[responseSize];
            new Random().NextBytes(requestData);

            Task<List<byte[]>> responder = ResponseAction(responseData, requesterSocket, responderSocket);
            byte[] response = testclass.Request(request);
            responder.Wait();

            Assert.AreEqual(responseData, response);
            //Assert.AreEqual(input, responderSocket.RecieveData);
        }

        /// <summary>
        /// Test that the socket will be able to handel data with can fit into the buffer but isn't in the buffer because the 
        /// recieved data is less than the total length of the data
        /// </summary>
        [Test]
        public void LowInitialRecieveSize()
        {
            PairedSocket requesterSocket = new PairedSocket();
            PairedSocket responderSocket = new PairedSocket(requesterSocket);
            responderSocket.SubDivideRecieveData = Constants.HubBufferSize / 3;
            IRequester testclass = CreateRequester(responderSocket);

            byte[] responseData;
            byte[] input = SocketResponderTests.BuildRandomRequest(Constants.HubBufferSize - 70, out responseData);
            byte[] requestData = new byte[Constants.HubBufferSize - 70];
            new Random().NextBytes(requestData);

            Task<List<byte[]>> responder = ResponseAction(responseData, requesterSocket, responderSocket);
            byte[] response = testclass.Request(CameraRequest.Alive);
            responder.Wait();

            Assert.AreEqual(responseData, response);

            //Assert.AreEqual(input, responderSocket.RecieveData);
        }

        async Task<List<byte[]>> ResponseAction(byte[] data, PairedSocket requesterSocket, PairedSocket responderSocket)
        {
            IResponder responder = CreateResponder(requesterSocket);
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
