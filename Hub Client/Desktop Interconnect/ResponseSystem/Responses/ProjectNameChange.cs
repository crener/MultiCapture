using System;
using System.Collections.Generic;
using System.Text;
using Hub.ResponseSystem;
using Hub.Util;

namespace Hub.DesktopInterconnect.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.ProjectNameChange)]
    class ProjectNameChange : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            int projectid;
            byte[] response;

            if (!ExtractParameter(command, parameters, "id", out projectid, out response))
                return response;

            if (!hasParameter(command, parameters, "set", out response))
                return response;

            try
            {
                ProjectMapper project = ProjectCache.RetrieveProject(projectid);
                Console.WriteLine("Project name changed from \"{0}\" to \"{1}\"", project.Name, parameters["name"]);
                project.saveData.ProjectName = parameters["name"];
                project.Save();

                if (Deployer.ProjectManager != null)
                    Deployer.ProjectManager.RefreshProject(projectid);
                return ResponseConstants.SuccessResponse;
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine(command + "could not find project " + projectid);
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?project could not be found! project: " +
                                               projectid);
            }
        }
    }
}
