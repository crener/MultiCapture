using System;
using Hub.DesktopInterconnect;

namespace Hub.ResponseSystem
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