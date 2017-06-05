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
        protected int imagesetId = -1;

        internal GenericManager(SaveLoad.Data config)
        {
            this.config = config;

            Random rand = new Random();
            int projectId = rand.Next(0, int.MaxValue - 1);
            SavePath = Constants.DefaultHubSaveLocation() + "Project" + projectId +
                           Path.DirectorySeparatorChar;

            //check that this new path has an existing path with no files in it
            bool done = false;
            do
            {
                if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                else if (Directory.GetFiles(savePath).Length > 0)
                {
                    Console.WriteLine(
                        "Randomly generated project directory (id: " + projectId + ") contains files, trying another!");

                    projectId = rand.Next(int.MaxValue, 0);
                    savePath = Constants.DefaultHubSaveLocation() + "Project" + projectId + Path.DirectorySeparatorChar;
                }
                else done = true;
            } while (!done);

            Console.WriteLine("Project directory generated, id: " + projectId);
            projectFile = new ProjectMapper(savePath + "project.xml", projectId);
            projectFile.Save();

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
                imagesetId = 0;

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
