using System;

namespace Hub.DesktopInterconnect.Responses
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ResponseTypeAttribute : Attribute
    {
        public ScannerCommands Response { get; }

        public ResponseTypeAttribute(ScannerCommands response)
        {
            Response = response;
        }
    }
}