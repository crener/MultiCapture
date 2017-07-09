using System;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public class ResponderException : Exception
    {
        public ResponderException() : base() { }
        public ResponderException(string message ) : base(message) { }
    }

    public class ResponseNeededException : ResponderException
    {
        public ResponseNeededException() : base() { }
        public ResponseNeededException(string message ) : base(message) { }
    }

    public class NoRequestException : ResponderException
    {
        public NoRequestException() : base() { }
        public NoRequestException(string message ) : base(message) { }
    }

    public class SocketDisconnectedException : ResponderException
    {
        public SocketDisconnectedException() : base() { }
        public SocketDisconnectedException(string message ) : base(message) { }
    }

    public class SocketNotConnectedException : ResponderException
    {
        public SocketNotConnectedException() : base() { }
        public SocketNotConnectedException(string message ) : base(message) { }
    }
}
