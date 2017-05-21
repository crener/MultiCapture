using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SharedDeviceItems;
using Hub.Helpers;
using Hub.Networking;

namespace Hub.Threaded
{
    internal class CameraThread : ICameraThread
    {
        public bool Finish { get { return finish; } set { finish = value; } }
        private volatile bool finish = false;

        public CameraRequest Request { get { return request; } set { request = value; } }
        private volatile CameraRequest request = CameraRequest.Alive;

        public string SavePath { get { return savePath; } set { savePath = value; } }
        public volatile string savePath;

        public string ImageSetName { get; set; }

        private CameraSocket config;
        private INetwork connection;

        public CameraThread(CameraSocket socket)
        {
            config = socket;
            connection = new SynchronousNet(socket.DataSocket);
            savePath = Constants.DefaultHubSaveLocation();
        }

        public CameraThread(CameraSocket socket, string saveLocation)
        {
            config = socket;
            connection = new SynchronousNet(socket.DataSocket);
            savePath = saveLocation;
        }

        public void Start()
        {
            try
            {
                SetCameraProperties();

                while (!Finish)
                {
                    if (Request != CameraRequest.Alive)
                    {
                        ProcessRequest(Request);
                        Request = CameraRequest.Alive;
                    }

                    Thread.Sleep(5);
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Thread Aborting (Because of exception)");
            }
            finally
            {
                Shutdown();
            }
        }

        /// <summary>
        /// Close all sensitive parts of the thread that may cause problems later
        /// </summary>
        private void Shutdown()
        {
            try
            {
                config.DataSocket.Shutdown(SocketShutdown.Both);
                config.DataSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("While Trying to close socket something went wrong... " + e.Message);
                Console.WriteLine(e);
            }
        }

        private void ProcessRequest(CameraRequest camRequest)
        {
            //start asking the camera for a new image
            byte[] data = connection.MakeRequest(PropertyRequest(camRequest));

            //extract image data
            string imageName;
            byte[] imageData;
            ByteManipulation.SeparateData(out imageName, data, out imageData);
            if (imageName == "" || imageData.Length <= 0)
            {
                Console.WriteLine("No Image data received!!");
                Console.WriteLine("Debug data:");
                Console.WriteLine("\tThread Camera: " + config.Config.Id);
                Console.WriteLine("\tImage set id: " + ImageSetName);
                Console.WriteLine("\tImage return string: " + Encoding.ASCII.GetString(data));
                return;
            }

            SaveData(imageData, savePath + Path.DirectorySeparatorChar + imageName);

            Console.WriteLine("Camera " + config.Config.Id + " image saved");
        }

        private void SaveData(byte[] data, string location)
        {
            using (FileStream fileStream = new FileStream(location, FileMode.CreateNew))
            {
                foreach (byte img in data)
                {
                    fileStream.WriteByte(img);
                }
            }
        }

        private byte[] PropertyRequest(CameraRequest camRequest)
        {
            CommandBuilder builder = new CommandBuilder().Request(camRequest);

            if (camRequest == CameraRequest.Alive || camRequest == CameraRequest.SendTestImage)
                return builder.Build();

            builder.AddParam("id", ImageSetName);

            return builder.Build();
        }

        /// <summary>
        /// Sets the camera properties
        /// </summary>
        private void SetCameraProperties()
        {
            CommandBuilder builder = new CommandBuilder().Request(CameraRequest.SetProporties);
            builder.AddParam("name", config.Config.CamFileIdentity);
            builder.AddParam("id", "0");

            connection.MakeRequest(builder.Build());
        }

        /// <summary>
        /// use when debugging - clears socket buffer of data
        /// </summary>
        public void ClearSockets()
        {
            byte[] ignore = new byte[300];
            int total = 0;
            while (config.DataSocket.Available > 0)
            {
                total += config.DataSocket.Receive(ignore);
            }
#if DEBUG
            Console.WriteLine("Total bytes flushed: " + total);
#endif
        }
    }
}
