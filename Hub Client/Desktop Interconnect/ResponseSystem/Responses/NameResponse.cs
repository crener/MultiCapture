using System;
using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Helpers;
using Hub.Util;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.SetName)]
    internal class NameResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            byte[] response;
            if (!hasParameter(command, parameters, "name", out response))
                return response;

            SaveLoad.Data newConf = Deployer.SysConfig;
            Console.WriteLine("Scanner Name was: {0}", newConf.name);
            newConf.name = parameters["name"];
            Deployer.SysConfig = newConf;

            Console.WriteLine("Scanner Name updated to: {0}", parameters["name"]);
            return ResponseConstants.SuccessResponse;
        }
    }
}
