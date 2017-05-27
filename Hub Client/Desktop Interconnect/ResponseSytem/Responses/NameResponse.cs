using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.DesktopInterconnect.Responses;
using Hub.Helpers;
using Hub.Util;

namespace Hub.DesktopInterconnect.Responses
{
    [ResponseType(ScannerCommands.setName)]
    class NameResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("name"))
            {
                Console.WriteLine("Name change instruction missing parameter: name");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"name\" parameter missing");
            }

            SaveLoad.Data newConf = Deployer.SysConfig;
            newConf.name = parameters["name"];
            Deployer.SysConfig = newConf;

            Console.WriteLine("Scanner Name updated to: {0}", parameters["name"]);
            return ResponseConstants.SuccessResponse;
        }

        public override void Reset()
        {
        }
    }
}
