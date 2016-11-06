using System;

namespace SharedDeviceItems.Exceptions
{
    public class CaptureFailedException : Exception
    {
        public CaptureFailedException()
            : base()
        {
        }

        public CaptureFailedException(string message)
            : base(message)
        {
        }

        public CaptureFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
