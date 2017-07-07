using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string Json { get { return JsonConvert.SerializeObject(project); } }

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

        private void Save()
        {
            string path = SharedDeviceItems.Constants.DefaultHubSaveLocation() + fileName;
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(Json);
            }
        }

        public void RemoveProject(int projectId)
        {
            ProjectInfo remove;

            try
            {
                remove = project.First(p => p.Id == projectId);
            }
            catch (InvalidOperationException)
            {
                //project not found
                Console.WriteLine("Project Manger attempted to remove project " + projectId + " could not be found");
                return;
            }

            project.Remove(remove);
        }

        public bool ProjectExists(int projectId)
        {
            try
            {
                project.First(p => p.Id == projectId);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return true;
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
                Name = data.ProjectName ?? data.ProjectId.ToString();
                Id = data.ProjectId;

                foreach (ProjectMapper.ImageSet imageSet in data.sets)
                {
                    foreach (ProjectMapper.Image image in imageSet.Images)
                    {
                        ImageCount++;
                        if (File.Exists(ProjectMapper.AbsoluteImagePath(data, imageSet.ImageSetId, image.File)))
                            SavedCount++;
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
