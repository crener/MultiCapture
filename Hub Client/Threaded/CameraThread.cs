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

        private CameraSocket config;
        private INetwork connection;

        public CameraThread(CameraSocket socket)
        {
            config = socket;
            connection = new SynchronousNet(socket.DataSocket);
        }

        public void Start()
        {
            try
            {
                while (!Finish)
                {
                    if (Request != CameraRequest.Alive)
                    {
                        ProcessRequest(Request);
                        Request = CameraRequest.Alive;
                    }

                    Thread.Sleep(10);
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
            byte[] data = connection.MakeRequest(Request);

            //extract image data
            string imageName;
            byte[] imageData;
            ByteManipulation.SeperateData(out imageName, data, out imageData);
            if (imageName == "" || imageData.Length <= 0)
            {
                Console.WriteLine("No Image data recieved!!");
                return;
            }

            using (FileStream fileStream = new FileStream(Path.DirectorySeparatorChar + "scanimage" + imageName, FileMode.CreateNew))
            {
                for (int i = 0; i < imageData.Length; i++)
                {
                    fileStream.WriteByte(imageData[i]);
                }
            }
        }
    }
}
