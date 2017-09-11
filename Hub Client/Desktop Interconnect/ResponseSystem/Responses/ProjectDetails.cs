using System;
using System.Collections.Generic;
using System.Text;
using Hub.ResponseSystem;
using Hub.Util;
using Newtonsoft.Json;

namespace Hub.DesktopInterconnect.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.ProjectDetails)]
    internal class ProjectDetails : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            int projectid;
            byte[] response;

            if (!ExtractParameter(command, parameters, "id", out projectid, out response))
                return response;

            try
            {
                ProjectMapper project = ProjectCache.RetrieveProject(projectid);
                return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new ProjectJson(project)));
            }
            catch(KeyNotFoundException)
            {
                Console.WriteLine(command + "could not find project " + projectid);
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?project could not be found! project: " +
                                               projectid);
            }
        }

        private struct ProjectJson
        {
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public ProjectMapper.ImageSet[] ImageSets { get; set; }
            public ProjectMapper.Camera[] Cameras { get; set; }

            public ProjectJson(ProjectMapper project)
            {
                ProjectId = project.ProjectId;
                ProjectName = project.Name ?? ProjectId.ToString();
                ImageSets = project.saveData.sets.ToArray();
                Cameras = project.saveData.cameras.ToArray();
            }
        }
    }
}