using System.Collections.Generic;
using Hub.DesktopInterconnect;

namespace Hub.ResponseSystem
{
    /// <summary>
    /// Responsible to implementing a response for one or more ScannerCommands 
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Responds to the given scanner command
        /// </summary>
        /// <param name="command">request to respond too</param>
        /// <param name="parameters">additional parameters for the response</param>
        /// <returns>response in byte form</returns>
        byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters);

        /// <summary>
        /// reset to initial state in case of disconnect or unexpected error
        /// </summary>
        void Reset();
    }
}
