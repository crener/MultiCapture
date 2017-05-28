using System.Collections.Generic;
using Hub.DesktopInterconnect;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.getApiVersion)]
    class ApiResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            return ResponseConstants.ApiResponse;
        }

        public override void Reset()
        {
        }
    }
}
