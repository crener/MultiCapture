using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hub.Util;

namespace Hub.DesktopInterconnect
{
    /// <summary>
    /// Stores ProjectMappers and maintains a quick list of them for easy access by responses
    /// </summary>
    static class ProjectCache
    {
        private static Dictionary<int, ProjectMapper> projectCache = new Dictionary<int, ProjectMapper>();

        public static ProjectMapper RetrieveProject(int projectId)
        {
            if (projectCache.ContainsKey(projectId))
            {
                return projectCache[projectId];
            }
            else
            {
                bool found = SearchForProject(projectId);
                if (found) return projectCache[projectId];
            }

            throw new KeyNotFoundException("Project could not found");
        }

        /// <summary>
        /// loads project and adds it to the project cache
        /// </summary>
        /// <param name="projectId">the id of the requred project</param>
        /// <returns>true if the project could be loaded successfully</returns>
        private static bool SearchForProject(int projectId)
        {
            //check if the current project is wanted
            if (Deployer.Manager != null && Deployer.Manager.ProjectId == projectId)
                projectCache.Add(projectId, Deployer.Manager.ProjectData);

            //get the project via a more conventional method
            if (!Deployer.ProjectManager.ProjectExists(projectId)) return false;

            string path = SharedDeviceItems.Constants.DefaultHubSaveLocation() + projectId;
            if (!Directory.Exists(path)) return false;

            ProjectMapper wanted = new ProjectMapper(path, -1);
            if (wanted.saveData.ProjectId != projectId) return false;

            projectCache.Add(projectId, wanted);
            return true;
        }

        public static void ForceAddProjectMapper(int projectId, ProjectMapper projectData)
        {
            projectCache.Add(projectId, projectData);
        }

        public static void Clear()
        {
            projectCache.Clear();
        }
    }
}
