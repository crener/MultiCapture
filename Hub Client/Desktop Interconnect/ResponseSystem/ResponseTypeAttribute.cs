using System;
using Hub.DesktopInterconnect;

namespace Hub.ResponseSystem
{
    /// <summary>
    /// Attribute which signifies a ScannerCommand which a response will generate a response too
    /// </summary>
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