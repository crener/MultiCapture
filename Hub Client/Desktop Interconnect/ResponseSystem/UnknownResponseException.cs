using System;

namespace Hub.ResponseSystem
{
    /// <summary>
    /// Reponse isn't known to a class
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
