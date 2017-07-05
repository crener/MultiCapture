using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hub.Util
{
    /// <summary>
    /// Creates summaries for the state of all projects that are stored on device.
    /// required by response system for project state information
    /// </summary>
    public class ProjectManager
    {
        List<ProjectInfo> project = new List<ProjectInfo>();
        private const string fileName = "summary.json";

        public ProjectManager()
        {
            RecalculateData();
        }

        public void RecalculateData()
        {
            List<string> directories =
                new List<string>(Directory.EnumerateDirectories(SharedDeviceItems.Constants.DefaultHubSaveLocation()));

            foreach (string directory in directories)
            {
                if (directory.EndsWith(fileName) || directory.EndsWith("Log")) continue;

                string dataPath = directory + Path.DirectorySeparatorChar + ProjectMapper.FileName;
                if (!File.Exists(dataPath)) continue;

                ProjectMapper.Data statistics = ProjectMapper.ExtractSaveData(dataPath);
                ProjectInfo info = new ProjectInfo(statistics);

                project.Add(info);
            }

            Save();
        }

        public void Save()
        {
            string path = SharedDeviceItems.Constants.DefaultHubSaveLocation() + Path.DirectorySeparatorChar + fileName;
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(JsonConvert.SerializeObject(project));
            }
        }

        private class ProjectInfo
        {
            public ProjectInfo()
            {

            }

            public ProjectInfo(ProjectMapper mapper) : this(mapper.saveData)
            {
            }

            public ProjectInfo(ProjectMapper.Data data)
            {
                Name = data.ProjectName;
                Id = data.ProjectId;

                foreach (ProjectMapper.ImageSet imageSet in data.sets)
                {
                    foreach (ProjectMapper.Image image in imageSet.images)
                    {
                        ImageCount++;
                        if (File.Exists(image.File)) SavedCount++;
                    }
                }
            }

            public string Name { get; set; }
            public int Id { get; set; }
            public int ImageCount { get; set; }
            public int SavedCount { get; set; }
        }
    }
}
