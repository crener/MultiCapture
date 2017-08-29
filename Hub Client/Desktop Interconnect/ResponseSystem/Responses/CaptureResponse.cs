using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hub.DesktopInterconnect;
using Hub.Util;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.CaptureImageSet)]
    internal class CaptureResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            Console.WriteLine("Network capture");
            Deployer.Manager.CaptureImageSet();

            return ResponseConstants.SuccessResponse;
        }
    }
}
