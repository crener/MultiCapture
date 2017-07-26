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
            if (!parameters.ContainsKey("id"))
            {
                Console.WriteLine(command + " is missisng parameter: id");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"id\" parameter is missing");
            }

            if (!parameters.ContainsKey("name"))
            {
                Console.WriteLine(command + " is missisng parameter: name");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"name\" parameter is missing");
            }

            int projectid;
            bool success = int.TryParse(parameters["id"], out projectid);

            if (!success)
            {
                Console.WriteLine(command + " could't convert parameter: id");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"id\" could not be converted. Is \"" +
                                               parameters["id"] + "\" valid?");
            }

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
