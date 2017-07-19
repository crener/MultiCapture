using System;
using System.IO;
using Hub.Helpers;
using Hub.Helpers.Interface;
using Hub.Util;
using SharedDeviceItems;

namespace Hub.Threaded
{
    abstract class GenericManager : ICameraManager
    {
        protected SaveLoad.Data config;
        protected ProjectMapper projectFile;

        protected string savePath;
        public static int ImagesetId { get; protected set; }
        public int ProjectId { get; }
        public ProjectMapper ProjectData { get { return projectFile; } }

        internal GenericManager(SaveLoad.Data config)
        {
            this.config = config;
            ImagesetId = -1;

            Random rand = new Random();
            ProjectId = rand.Next(0, int.MaxValue - 1);
            SavePath = Constants.DefaultHubSaveLocation() + ProjectId +
                           Path.DirectorySeparatorChar;

            //check that this new path has an existing path with no files in it
            bool done = false;
            do
            {
                if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                else if (Directory.GetFiles(savePath).Length > 0)
                {
                    Console.WriteLine(
                        "Randomly generated project directory (id: " + ProjectId + ") contains files, trying another!");

                    ProjectId = rand.Next(int.MaxValue, 0);
                    savePath = Constants.DefaultHubSaveLocation() + ProjectId + Path.DirectorySeparatorChar;
                }
                else done = true;
            } while (!done);

            Console.WriteLine("Project directory generated, id: " + ProjectId);
            projectFile = new ProjectMapper(savePath + ProjectMapper.FileName, ProjectId);
            projectFile.Save();
            Deployer.CurrentProject = projectFile;

            Configure();
        }

        /// <summary>
        /// configration for implementation specific items
        /// </summary>
        protected abstract void Configure();

        public string SavePath
        {
            set
            {
                savePath = value;

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                //since the directory has changed the image set should change too
                ImagesetId = 0;

                //implementation specific modifications to internal data
                SaveChange(value);
            }
            get
            {
                return savePath;
            }
        }

        protected abstract void SaveChange(string value);

        public void CaptureImageSet()
        {
            CaptureImageSet(CameraRequest.SendFullResImage);
        }

        public abstract void CaptureImageSet(CameraRequest wanted);

#if DEBUG
        public abstract void ClearSockets();
#endif
    }
}
