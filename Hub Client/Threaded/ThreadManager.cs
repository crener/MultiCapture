using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Hub.Helpers;
using SharedDeviceItems;
using SharedDeviceItems.Networking;

namespace Hub.Threaded
{
    [Obsolete("Use the TaskManager Instead", false)]
    class ThreadManager : GenericManager
    {
        private Thread[] cameraThreads;
        private ICameraThread[] threadConfiguration;
        

        public ThreadManager(SaveLoad.Data config) : base(config)
        {

        }

#if DEBUG
        /// <summary>
        /// Use for debug to pass mock camera tasks
        /// </summary>
        /// <param name="config">standard config for initialization</param>
        /// <param name="tasks">Cameras that will be used for processing</param>
        public ThreadManager(SaveLoad.Data config, ICameraThread[] tasks) : base(config)
        {
            threadConfiguration = tasks;
        }
#endif

        /// <summary>
        /// Close all the threads proporly making sure to "force quit" them if they are being strnage
        /// </summary>
        ~ThreadManager()
        {
            for (int i = 0; i < cameraThreads.Length; i++)
                threadConfiguration[i].Finish = true;

            Thread.Sleep(50);

            foreach(Thread thread in cameraThreads)
                thread.Join();

            foreach (Thread thread in cameraThreads)
                if (thread.IsAlive) thread.Abort();
        }

        public override void CaptureImageSet(CameraRequest wanted)
        {
            if (!CheckDone()) return;

            //iterate the image identifier name
            if (wanted == CameraRequest.SendFullResImage)
            {
                ++imagesetId;
                Directory.CreateDirectory(savePath + Path.DirectorySeparatorChar + "set-" + imagesetId);

                SaveChange(savePath);
            }

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

        /// <summary>
        /// Initialize the threads
        /// </summary>
        protected override void Configure()
        {
            List<Thread> localCameraThreads = new List<Thread>();
            List<CameraThread> localThreadConfiguration = new List<CameraThread>();

            for (int i = 0; i < config.CameraCount; i++)
            {
                //check that the camera is available
                CameraSocket tempCameraSockets = new CameraSocket
                {
                    DataSocket = new SocketWrapper(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                    Config = config.Cameras[i]
                };

                if (tempCameraSockets.Setup())
                {
                    Console.WriteLine("Connected to camera " + config.Cameras[i].Id + " assigning a thread");
                    CameraThread threadTask = new CameraThread(tempCameraSockets, savePath);
                    Thread tempThread = new Thread(threadTask.Start) { Name = config.Cameras[i].CamFileIdentity };
                    tempThread.Start();

                    localThreadConfiguration.Add(threadTask);
                    localCameraThreads.Add(tempThread);
                    projectFile.AddCamera(i, localCameraThreads[i].Name);
                }
                else
                {
                    Console.WriteLine("Failed to connect to camera " + config.Cameras[i].Id + "!!");
                }
            }

            threadConfiguration = localThreadConfiguration.ToArray();
            cameraThreads = localCameraThreads.ToArray();
        }
        
        protected override void SaveChange(string value)
        {
            if(threadConfiguration == null) return;

            foreach (ICameraThread thread in threadConfiguration)
                thread.SavePath = value + "set-" + imagesetId;
        }

        private bool CheckDone()
        {
            if (threadConfiguration == null) return true;

            foreach (ICameraThread thread in threadConfiguration)
            {
                if (thread.Request != CameraRequest.Alive)
                    return false;
            }
            return true;
        }

#if DEBUG
        /// <summary>
        /// use when debugging - clears every socket buffer of data
        /// </summary>
        public override void ClearSockets()
        {
            foreach (CameraThread cam in threadConfiguration)
                cam.ClearSockets();

            Console.WriteLine("Cleared Sockets");
        }
#endif
    }
}
