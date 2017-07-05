using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Util
{
    /// <summary>
    /// Creates summaries for the state of all projects that are stored on device.
    /// required by response system for project state information
    /// </summary>
    public class ProjectManager
    {
        List<projectInfo> project = new List<projectInfo>();
        private const string fileName = "summary.json";

        public ProjectManager()
        {
            RecalculateData();
        }

        public void RecalculateData()
        {
            List<string> directories =
                new List<string>(Directory.EnumerateDirectories(SharedDeviceItems.Constants.DefaultHubSaveLocation()));

            foreach(string directory in directories)
            {
                if(directory.EndsWith(fileName) || directory.EndsWith("Log")) continue;

                ProjectMapper project = new ProjectMapper(directory, 1);
            }
        }

        private class projectInfo
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public int ImageCount { get; set; }
            public int SavedCount { get; set; }
        }
    }
}
