using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub.Helpers.Interface;
using Hub.Helpers.Wrapper;
using Hub.Util;
using SharedDeviceItems;
using static Hub.Helpers.CameraHelper;

namespace Hub.Threaded
{
    class TaskManager : ICameraManager
    {
        private SaveContainer.Data config;
        private ICameraTask[] cameras;
        private ProjectMapper projectFile;

        //proporties
        private int imagesetId = -1;
        private string savePath;

        public TaskManager(SaveContainer.Data config)
        {
            this.config = config;

            Random rand = new Random();
            int projectId = rand.Next(0, int.MaxValue - 1);
            savePath = Constants.DefualtHubSaveLocation() + "Project" + projectId +
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
                    savePath = Constants.DefualtHubSaveLocation() + "Project" + projectId + Path.DirectorySeparatorChar;
                }
                else done = true;
            } while (!done);

            Console.WriteLine("Project directory generated, id: " + projectId);
            projectFile = new ProjectMapper(savePath + "project.xml", projectId);
            projectFile.Save();

            Configure();
        }

        ~TaskManager()
        {
            ShutDownCameras();
        }

        public void CaptureImageSet()
        {
            CaptureImageSet(CameraRequest.SendFullResImage);
        }

        public void CaptureImageSet(CameraRequest wanted)
        {
            Console.WriteLine("Start of image capture, request: " + wanted + ", imageSet: " + (imagesetId + 1));
            Task[] camCommand = CollectImageCommands(wanted);
            foreach(Task task in camCommand) task.Start();

            projectFile.AddImageSet(imagesetId, "set-" + imagesetId);
            for (int i = 0; i < config.Cameras.Length; i++)
            {
                projectFile.AddImage(imagesetId, config.Cameras[i].CamFileIdentity + imagesetId + ".jpg", i);
            }
            projectFile.Save();

            Task.WaitAll(camCommand);
            Console.WriteLine("Image Requests Complete");
        }

        private Task[] CollectImageCommands(CameraRequest request)
        {
            Task[] result = new Task[cameras.Length];
            bool needsStoreLocation = SavesImage(request);

            if (needsStoreLocation)
            {
                //ensure each image has unique name and place to store image
                ++imagesetId;
                Directory.CreateDirectory(savePath + Path.DirectorySeparatorChar + "set-" + imagesetId);
            }

            for (int i = 0; i < cameras.Length; i++)
            {
                if(needsStoreLocation)
                {
                    cameras[i].SavePath = savePath + "set-" + imagesetId;
                    cameras[i].ImageSetName = imagesetId.ToString();
                }

                result[i] = cameras[i].ProcessRequest(request);
            }

            return result;
        }

        /// <summary>
        /// Iniitialises resources for ThreadManager
        /// </summary>
        private void Configure()
        {
            List<ICameraTask> threadConfiguration = new List<ICameraTask>();

            for (int i = 0; i < config.CameraCount; i++)
            {
                //check that the camera is avalible
                CameraSocket tempCameraSockets = new CameraSocket
                {
                    DataSocket = new WSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                    Config = config.Cameras[i]
                };

                if (tempCameraSockets.Setup())
                {
                    Console.WriteLine("Connected to camera " + config.Cameras[i].Id);
                    ICameraTask threadTask = new CameraTask(tempCameraSockets, savePath);

                    threadConfiguration.Add(threadTask);
                    projectFile.AddCamera(i, config.Cameras[i].CamFileIdentity);
                }
                else
                {
                    Console.WriteLine("Failed to connect to camera " + config.Cameras[i].Id + "!!");
                }
            }

            cameras = threadConfiguration.ToArray();
            projectFile.Save();
        }

        public string SavePath
        {
            set
            {
                savePath = value;

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                //since the directory has changed the image set should change too
                imagesetId = 0;

                foreach (ICameraTask thread in cameras)
                    thread.SavePath = value + "set-" + imagesetId;
            }
            get
            {
                return savePath;
            }
        }

        /// <summary>
        /// Close all the threads proporly making sure to "force quit" them if they are being strnage
        /// </summary>
        protected void ShutDownCameras()
        {
            foreach (ICameraTask camera in cameras)
            {
                camera.ShutDown();
            }
        }

        /// <summary>
        /// use when debugging - clears every socket buffer of data
        /// </summary>
        public void ClearSockets()
        {
            foreach (ICameraTask cam in cameras)
                cam.ClearSockets();

            Console.WriteLine("Cleared Sockets");
        }
    }
}
