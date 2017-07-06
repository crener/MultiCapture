using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Util;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.CurrentProjects)]
    class SavedProjectsResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            return Encoding.ASCII.GetBytes(Deployer.ProjectManager.Json);
        }
    }
}
