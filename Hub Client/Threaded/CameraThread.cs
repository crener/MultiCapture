using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using SharedDeviceItems;
using Hub.Helpers;
using Hub.Networking;

namespace Hub.Threaded
{
    public class CameraThread
    {
        public volatile bool Finish = false;
        public volatile CameraRequest Request = CameraRequest.Alive;

        public string ImageSetName {  get; set; }
        public string savePath { get; set; }

        private CameraSocket config;
        private INetwork connection;

        public CameraThread(CameraSocket socket)
        {
            config = socket;
            connection = new SynchronousNet(socket.DataSocket);
            savePath = Path.DirectorySeparatorChar + "scanimage";
        }

        public void Start()
        {
            try
            {
                SetCameraProporties();

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

        private void ProcessRequest(CameraRequest request)
        {
            //start asking the camera for a new image
            byte[] data = connection.MakeRequest(PropertyRequest(request));

            //extract image data
            string imageName;
            byte[] imageData;
            ByteManipulation.SeperateData(out imageName, data, out imageData);
            if (imageName == "" || imageData.Length <= 0)
            {
                Console.WriteLine("No Image data recieved!!");
                return;
            }

            using (FileStream fileStream = new FileStream(savePath + imageName, FileMode.CreateNew))
            {
                for (int i = 0; i < imageData.Length; i++)
                {
                    fileStream.WriteByte(imageData[i]);
                }
            }
        }

        private byte[] PropertyRequest(CameraRequest request)
        {
            CommandBuilder builder = new CommandBuilder().Request(request);

            if(request == CameraRequest.Alive || request == CameraRequest.SendTestImage)
                return builder.Build();

            builder.AddParam("id", ImageSetName);

            return builder.Build();
        }

        /// <summary>
        /// Sets the camera proporties
        /// </summary>
        private void SetCameraProporties()
        {
            CommandBuilder builder = new CommandBuilder().Request(CameraRequest.SetProporties);
            builder.AddParam("name", config.Config.CamFileIdentity);
            builder.AddParam("id", "0");

            connection.MakeRequest(builder.Build());
        }
    }
}
