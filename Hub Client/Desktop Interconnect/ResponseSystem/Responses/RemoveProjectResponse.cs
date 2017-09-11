using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Util;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.RemoveProject)]
    class RemoveProjectResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            int project = -1;
            byte[] response;

            if (!ExtractParameter(command, parameters, "id", out project, out response))
                return response;

            string path = SharedDeviceItems.Constants.DefaultHubSaveLocation() + parameters["id"];

            if (!Directory.Exists(path) || !Deployer.ProjectManager.ProjectExists(project))
            {
                Console.WriteLine(command + " project not found, cannot remove");
#if DEBUG
                Console.WriteLine(command + "\t projectID: " + parameters["id"]);
#endif
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?Project " + project +
                                               " not found");
            }

            if (Deployer.Manager.ProjectId == project)
            {
                Console.WriteLine(command + " wanted to remove the current project");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?You cannot remove the current project!");
            }

            Directory.Delete(path, true);
            Deployer.ProjectManager.RemoveProject(project);

            return ResponseConstants.SuccessResponse;
        }
    }
}
