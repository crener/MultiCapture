using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub.Threaded.Cameras;
using SharedDeviceItems;
using SharedDeviceItems.Networking;
using static Hub.Helpers.CameraHelper;

[assembly: InternalsVisibleTo("Hub ClientTests")]
namespace Hub.Threaded
{
    internal class TaskManager : GenericManager
    {
        private ICameraTask[] cameras;


        public TaskManager(SaveLoad.Data config) : base(config)
        {

        }

#if DEBUG
        /// <summary>
        /// Use for debug to pass mock camera tasks
        /// </summary>
        /// <param name="config">standard config for initialization</param>
        /// <param name="tasks">Cameras that will be used for processing</param>
        public TaskManager(SaveLoad.Data config, ICameraTask[] tasks) : base(config)
        {
            cameras = tasks;
        }
#endif

        /// <summary>
        /// Close all the threads proporly making sure to "force quit" them if they are being strange
        /// </summary>
        ~TaskManager()
        {
            foreach (ICameraTask camera in cameras)
            {
                camera.Dispose();
            }
        }

        public override void CaptureImageSet(CameraRequest wanted)
        {
            Console.WriteLine("Start of image capture, request: " + wanted + ", imageSet: " + (ImagesetId + 1));
            Task[] camCommand = CollectImageCommands(wanted);
            foreach (Task task in camCommand) task.Start();

            projectFile.AddImageSet(ImagesetId, "set-" + ImagesetId);

            Task.WaitAll(camCommand);
            projectFile.Save();
            Console.WriteLine("Image Requests Complete");
        }

        private Task[] CollectImageCommands(CameraRequest request)
        {
            Task[] result = new Task[cameras.Length];
            bool needsStoreLocation = SavesImage(request);

            if (needsStoreLocation)
            {
                //ensure each image has unique name and place to store image
                ++ImagesetId;
                Directory.CreateDirectory(savePath + Path.DirectorySeparatorChar + "set-" + ImagesetId);
            }

            for (int i = 0; i < cameras.Length; i++)
            {
                if (needsStoreLocation)
                {
                    cameras[i].SavePath = savePath + "set-" + ImagesetId;
                }

                result[i] = cameras[i].ProcessRequest(request);
            }

            return result;
        }

        /// <summary>
        /// Initializes resources for ThreadManager
        /// </summary>
        protected override void Configure()
        {
            List<ICameraTask> threadConfiguration = new List<ICameraTask>();

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
                    Console.WriteLine("Connected to camera " + config.Cameras[i].Id);
                    ICameraTask threadTask = new CameraTask(tempCameraSockets, savePath);

                    threadConfiguration.Add(threadTask);
                    projectFile.AddCamera(config.Cameras[i].Id, config.Cameras[i].CamFileIdentity);
                }
                else
                {
                    Console.WriteLine("Failed to connect to camera " + config.Cameras[i].Id + "!!");
                    Console.WriteLine("\tPort:\t\t " + config.Cameras[i].Port);
                    Console.WriteLine("\tLong addr:\t " + config.Cameras[i].Address);
                    Console.WriteLine("\tFile ID:\t " + config.Cameras[i].CamFileIdentity);
                }
            }

            if(config.enableInternalCamera)
            {
                threadConfiguration.Add(new InternalCameraTask(config));
                projectFile.AddCamera(config.internalCameraId, "Hub");
                Console.WriteLine("Hub Camera Initialized");
            }

            cameras = threadConfiguration.ToArray();
            projectFile.Save();
        }

        protected override void SaveChange(string value)
        {
            if(cameras == null || cameras.Length == 0) return;

            foreach (ICameraTask cam in cameras)
                cam.SavePath = value + "set-" + ImagesetId;
        }

#if DEBUG
        /// <summary>
        /// use when debugging - clears every socket buffer of data
        /// </summary>
        public override void ClearSockets()
        {
            foreach (ICameraTask cam in cameras)
                cam.ClearSockets();

            Console.WriteLine("Cleared Sockets");
        }
#endif
    }
}
