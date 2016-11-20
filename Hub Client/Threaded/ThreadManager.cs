using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Hub.Helpers;
using Hub.Helpers.Wrapper;
using SharedDeviceItems;

namespace Hub.Threaded
{
    class ThreadManager
    {
        private SaveContainer.Data config;
        private Thread[] cameraThreads;
        private CameraThread[] threadConfiguration;
        private CameraSocket[] cameraSockets;

        //proporties
        private int imagesetId = -1;
        private string savePath = Path.DirectorySeparatorChar + "scanimage";

        public ThreadManager(SaveContainer.Data config)
        {
            this.config = config;
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
            UpdateCameraParams(wanted);

            for (int i = 0; i < cameraThreads.Length; i++)
            {
                if (cameraThreads[i] != null && cameraThreads[i].IsAlive)
                {
                    threadConfiguration[i].ImageSetName = imagesetId.ToString();
                    threadConfiguration[i].Request = wanted;
                }
            }
        }

        public void UpdateCameraParams(CameraRequest image)
        {
            if (image == CameraRequest.Alive ||
               image == CameraRequest.SendTestImage ||
               image == CameraRequest.SetProporties)
                return;

            //iterate the image identifier name
            if (image == CameraRequest.SendFullResImage)
            {
                ++imagesetId;
            }
        }

        /// <summary>
        /// Initialise the threads
        /// </summary>
        private void ConfigureThreads()
        {
            cameraSockets = new CameraSocket[config.CameraCount];
            cameraThreads = new Thread[config.CameraCount];
            threadConfiguration = new CameraThread[config.CameraCount];

            for (int i = 0; i < config.CameraCount; i++)
            {
                //check that the camera is avalible
                cameraSockets[i] = new CameraSocket
                {
                    DataSocket = new WSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                    Config = config.Cameras[i]
                };

                if (cameraSockets[i].Setup())
                {
                    Console.WriteLine("Connected to camera " + config.Cameras[i].Id + " assigning a thread");
                    CameraThread threadTask = new CameraThread(cameraSockets[i]);
                    threadConfiguration[i] = threadTask;
                    cameraThreads[i] = new Thread(threadTask.Start);
                    cameraThreads[i].Name = "CameraThread " + cameraSockets[i].Config.Id;
                    cameraThreads[i].Start();
                }
                else
                {
                    Console.WriteLine("Failed to connect to camera " + config.Cameras[i].Id + "!!");
                    cameraSockets[i] = null;
                }
            }
        }

        public string SavePath
        {
            set
            {
                savePath = value;

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                foreach (CameraThread thread in threadConfiguration)
                    thread.savePath = value;
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
    }
}
