using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Hub.Helpers;
using Hub.Helpers.Wrapper;
using Hub.Util;
using SharedDeviceItems;

namespace Hub.Threaded
{
    class ThreadManager
    {
        private SaveContainer.Data config;
        private Thread[] cameraThreads;
        private CameraThread[] threadConfiguration;
        private CameraSocket[] cameraSockets;
        private ProjectMapper projectFile;

        //proporties
        private int imagesetId = -1;
        private string savePath;

        public ThreadManager(SaveContainer.Data config)
        {
            this.config = config;

            Random rand = new Random();
            int projectId = rand.Next(0, int.MaxValue - 1);
            savePath = Constants.DefualtHubSaveLocation() + "Project" + projectId +
                           Path.DirectorySeparatorChar;
            bool done = false;

            //check that this new path has an existing path with no files in it
            do
            {
                if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                else if (Directory.GetFiles(savePath).Length > 0)
                {
                    Console.WriteLine(
                        "Randomly generated project directory (id: " + projectId + ") contains files, trying another!");

                    projectId = rand.Next(int.MaxValue, 0);
                    savePath = Constants.DefualtHubSaveLocation() + "tempProject" + projectId + Path.DirectorySeparatorChar;
                }
                else done = true;
            } while (!done);

            Console.WriteLine("Project directory generated, id: " + projectId);
            projectFile = new ProjectMapper(savePath + "project.xml", projectId);
            projectFile.Save();
            ConfigureThreads();
        }

        ~ThreadManager()
        {
            CloseThreads();
        }

        public void CaptureImageSet()
        {
            CaptureImageSet(CameraRequest.SendFullResImage);
        }

        public void CaptureImageSet(CameraRequest wanted)
        {
            if (!CheckDone()) return;

            UpdateCameraParams(wanted);

            for (int i = 0; i < cameraThreads.Length; i++)
            {
                if (cameraThreads[i] != null && cameraThreads[i].IsAlive)
                {
                    threadConfiguration[i].ImageSetName = imagesetId.ToString();
                    threadConfiguration[i].Request = wanted;
                }
            }

            projectFile.AddImageSet(imagesetId, "set-" + imagesetId);
            for (int i = 0; i < config.Cameras.Length; i++)
            {
                projectFile.AddImage(imagesetId, config.Cameras[i].CamFileIdentity + imagesetId + ".jpg", i);
            }
            projectFile.Save();
        }

        private void UpdateCameraParams(CameraRequest image)
        {
            //iterate the image identifier name
            if (image == CameraRequest.SendFullResImage)
            {
                ++imagesetId;

                Directory.CreateDirectory(savePath + Path.DirectorySeparatorChar + "set-" + imagesetId);

                foreach (CameraThread thread in threadConfiguration)
                    thread.SavePath = savePath + "set-" + imagesetId;
            }
        }

        /// <summary>
        /// Initialise the threads
        /// </summary>
        private void ConfigureThreads()
        {
            List<CameraSocket> cameraSockets = new List<CameraSocket>();
            List<Thread> cameraThreads = new List<Thread>();
            List<CameraThread> threadConfiguration = new List<CameraThread>();

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
                    Console.WriteLine("Connected to camera " + config.Cameras[i].Id + " assigning a thread");
                    CameraThread threadTask = new CameraThread(tempCameraSockets, savePath);
                    Thread tempThread = new Thread(threadTask.Start);
                    tempThread.Name = config.Cameras[i].CamFileIdentity;
                    tempThread.Start();

                    threadConfiguration.Add(threadTask);
                    cameraThreads.Add(tempThread);
                    cameraSockets.Add(tempCameraSockets);
                    projectFile.AddCamera(i, cameraThreads[i].Name);
                }
                else
                {
                    Console.WriteLine("Failed to connect to camera " + config.Cameras[i].Id + "!!");
                }
            }

            this.threadConfiguration = threadConfiguration.ToArray();
            this.cameraThreads = cameraThreads.ToArray();
            this.cameraSockets = cameraSockets.ToArray();
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

                foreach (CameraThread thread in threadConfiguration)
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
        protected void CloseThreads()
        {
            for (int i = 0; i < cameraThreads.Length; i++)
                threadConfiguration[i].Finish = true;

            Thread.Sleep(50);

            for (int i = 0; i < cameraThreads.Length; i++)
                cameraThreads[i].Join();

            foreach (Thread thread in cameraThreads)
                if (thread.IsAlive) thread.Abort();
        }

        private bool CheckDone()
        {
            foreach (CameraThread thread in threadConfiguration)
            {
                if (thread.Request != CameraRequest.Alive)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// use when debugging - clears every socket buffer of data
        /// </summary>
        public void ClearSockets()
        {
            foreach (CameraThread cam in threadConfiguration)
                cam.ClearSockets();

            Console.WriteLine("Cleared Sockets");
        }
    }
}
