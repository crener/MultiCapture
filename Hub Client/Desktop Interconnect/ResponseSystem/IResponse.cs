using System.Collections.Generic;
using Hub.DesktopInterconnect;

namespace Hub.ResponseSystem
{
    public interface IResponse
    {
        /// <summary>
        /// Responds to the given scanner command
        /// </summary>
        /// <param name="command">request to respond too</param>
        /// <returns>response in byte form</returns>
        byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters);

        /// <summary>
        /// reset to initial state in case of disconnect or unexpected error
        /// </summary>
        void Reset();
    }
}
