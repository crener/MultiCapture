using System;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public class InterconnectException : Exception
    {
        public InterconnectException() : base() { }
        public InterconnectException(string message ) : base(message) { }
    }

    public class ResponderException : InterconnectException
    {
        public ResponderException() : base() { }
        public ResponderException(string message ) : base(message) { }
    }

    public class RequesterException : InterconnectException
    {
        public RequesterException() : base() { }
        public RequesterException(string message ) : base(message) { }
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

    public class SocketDisconnectedException : InterconnectException
    {
        public SocketDisconnectedException() : base() { }
        public SocketDisconnectedException(string message ) : base(message) { }
    }

    public class SocketUnexpectedDataException : InterconnectException
    {
        public SocketUnexpectedDataException() : base() { }
        public SocketUnexpectedDataException(string message ) : base(message) { }
    }

    public class SocketNotConnectedException : InterconnectException
    {
        public SocketNotConnectedException() : base() { }
        public SocketNotConnectedException(string message ) : base(message) { }
    }
}
