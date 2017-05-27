using System;

namespace Hub.DesktopInterconnect.Responses
{
    /// <summary>
    /// Reponse isn't known to this class
    /// </summary>
    class UnknownResponseException : Exception
    {
        public UnknownResponseException(string message) : base(message)
        {
        }

        public UnknownResponseException() : 
            base("This calss doesn't know what to do for this response!")
        {
        }
    }
}
