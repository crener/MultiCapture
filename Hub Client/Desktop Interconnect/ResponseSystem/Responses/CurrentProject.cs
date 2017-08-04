using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Util;
using Newtonsoft.Json;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.CurrentProject)]
    class CurrentResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            return Encoding.ASCII.GetBytes(Deployer.CurrentProject.ProjectId.ToString());
        }
    }
}
