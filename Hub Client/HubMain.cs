using System;
using System.Net.Sockets;
using System.Threading;

using Hub.Camera;
using Hub.SaveLoad;
using Hub.Helpers;
using Hub.Threaded;
using SharedDeviceItems;

namespace Hub
{
    public class HubMain
    {
        private SaveContainer.Data config;
        private Thread[] cameraThreads;
        private CameraThread[] threadConfiguration;
        private CameraSocket[] cameraSockets;

        public static int Main(String[] args)
        {
            HubMain program = new HubMain();
            program.Start();
            program.Finish();
            return 0;
        }


        public HubMain()
        {
            config = SaveContainer.Load();

            Start();
        }

        /// <summary>
        /// Main runtime loop used to collect and send data off too the computer
        /// </summary>
        public void Start()
        {
            config = SaveContainer.Load();
            ConfigureThreads();
            while (Console.ReadLine() != "e")
            {
                CaptureImageSet();
            }
            Console.WriteLine("Quitting");
            Finish();
        }

        public void CaptureImageSet()
        {
            for (int i = 0; i < cameraThreads.Length; i++)
            {
                if (cameraThreads[i] != null && cameraThreads[i].IsAlive)
                {
                    threadConfiguration[i].Request = CameraRequest.SendTestImage;
                }
            }
        }

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
                    DataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
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


        /// <summary>
        /// Always do this to correctly end all actions currently being done
        /// </summary>
        private void Finish()
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