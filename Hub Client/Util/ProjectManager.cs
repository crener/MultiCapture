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

        /// <summary>
        /// calculate a summary of all the different projects
        /// </summary>
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
                ProjectInfo info;

                if(statistics == null) continue;

                if (Deployer.CurrentProject != null && statistics.ProjectId == Deployer.CurrentProject.ProjectId)
                    info = new DynamicProjectInfo(Deployer.CurrentProject);
                else info = new ProjectInfo(statistics);

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

        public void RefreshProject(int id)
        {
            //ensure the project exists
            if (!ProjectExists(id)) return;
            if (Deployer.CurrentProject.ProjectId == id) return;

            string projectPath = SharedDeviceItems.Constants.DefaultHubSaveLocation() + id + Path.DirectorySeparatorChar +
                                 ProjectMapper.FileName;
            if (!File.Exists(projectPath)) return;

            //find the project index
            int location = -1;
            for (int i = 0; i < project.Count; i++)
                if (project[i].Id == id)
                {
                    location = i;
                    break;
                }
            if (location == -1) return;

            project[location] = new ProjectInfo(ProjectMapper.ExtractSaveData(projectPath));
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
            public ProjectInfo(ProjectMapper mapper) : this(mapper.saveData)
            {
            }

            public ProjectInfo(ProjectMapper.Data data)
            {
                GenerateData(data);
            }

            protected void GenerateData(ProjectMapper.Data data)
            {
                Name = data.ProjectName ?? data.ProjectId.ToString();
                Id = data.ProjectId;

                foreach (ProjectMapper.ImageSet imageSet in data.sets)
                {
                    foreach (ProjectMapper.Image image in imageSet.Images)
                    {
                        imageCount++;
                        if (File.Exists(ProjectMapper.AbsoluteImagePath(data, imageSet.ImageSetId, image.File)))
                            savedCount++;
                    }
                }
            }

            public virtual string Name { get; set; }
            public int Id { get; set; }
            public virtual int ImageCount { get { return imageCount; } set { imageCount = value; } }
            public virtual int SavedCount { get { return savedCount; } set { savedCount = value; } }
            protected int imageCount, savedCount;
        }

        private class DynamicProjectInfo : ProjectInfo
        {
            private ProjectMapper mapper;
            private int lastSetCount;

            public DynamicProjectInfo(ProjectMapper mapper) : base(mapper.saveData)
            {
                this.mapper = mapper;
                lastSetCount = mapper.ImageSetCount;
            }

            private void CheckRegen()
            {
                if (lastSetCount != mapper.ImageSetCount)
                {
                    imageCount = savedCount = 0;

                    GenerateData(mapper.saveData);
                    lastSetCount = mapper.ImageSetCount;
                }
            }

            public override string Name => mapper.Name ?? mapper.ProjectId.ToString();

            public override int ImageCount
            {
                get
                {
                    CheckRegen();
                    return imageCount;
                }
                set { imageCount = value; }
            }

            public override int SavedCount
            {
                get
                {
                    CheckRegen();
                    return savedCount;
                }
                set { savedCount = value; }
            }
        }
    }
}
