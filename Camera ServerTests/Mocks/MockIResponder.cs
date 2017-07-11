using System;
using Hub.Networking;
using SharedDeviceItems.Exceptions;
using SharedDeviceItems.Networking.CameraHubConnection;

namespace CameraServerTests.Mocks
{
    class MockIResponder : IResponder
    {
        public int ThrowExceptionAfterRecieve { get; set; }
        public byte[] RecieveBytes { get; set; }
        public byte[] SendBytes { get; set; }

        public MockIResponder()
        {
            ThrowExceptionAfterRecieve = 1;
        }

        public void Connect(ISocket listeningSocket)
        {
        }

        public byte[] RecieveData()
        {
            --ThrowExceptionAfterRecieve;
            if(ThrowExceptionAfterRecieve < 0) throw new TestException();

            return RecieveBytes;
        }

        public void SendResponse(byte[] data)
        {
            SendBytes = data;
        }

        public int ClearSocket()
        {
            throw new NotImplementedException();
        }

        public bool Connected()
        {
            return ThrowExceptionAfterRecieve >= 0;
        }
    }
}
