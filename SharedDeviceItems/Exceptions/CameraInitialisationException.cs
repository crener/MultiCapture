using System;

namespace SharedDeviceItems.Exceptions
{
    public class CameraInitialisationException : Exception
    {
        public CameraInitialisationException()
            : base()
        {
        }

        public CameraInitialisationException(string message)
            : base(message)
        {
        }

        public CameraInitialisationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
