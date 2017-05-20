using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub.Networking;
using SharedDeviceItems;
using static Hub.Helpers.CameraHelper;

namespace Hub.Threaded
{
    class CameraTask : ICameraTask
    {
        public string ImageSetName { get; set; }
        public string SavePath { get; set; }
        private CameraSocket config;
        private INetwork connection;

        public CameraTask(CameraSocket socket)
        {
            config = socket;
            connection = new SynchronousNet(socket.DataSocket);
            SavePath = Constants.DefualtHubSaveLocation();
        }

        public CameraTask(CameraSocket socket, string saveLocation)
        {
            config = socket;
            connection = new SynchronousNet(socket.DataSocket);
            SavePath = saveLocation;
        }

        public Task ProcessRequest(CameraRequest request)
        {
            return new Task(() => InternalRequestProcess(request));
        }

        private void InternalRequestProcess(CameraRequest request)
        {
            //start asking the camera for a new image
            byte[] data = connection.MakeRequest(BuildExternalCommand(request));

            //extract image data
            string imageName;
            byte[] imageData;
            ByteManipulation.SeparateData(out imageName, data, out imageData);
            if (imageName == "" || imageData.Length <= 0)
            {
                Console.WriteLine("No Image data recieved!!");
                Console.WriteLine("Debug data:");
                Console.WriteLine("\tThread Camera: " + config.Config.Id);
                Console.WriteLine("\tImage set id: " + ImageSetName);
                Console.WriteLine("\tImage return string: " + Encoding.ASCII.GetString(data));
                return;
            }

            SaveData(imageData, SavePath + Path.DirectorySeparatorChar + imageName);

            Console.WriteLine("Camera " + config.Config.Id + " image saved");
        }

        private byte[] BuildExternalCommand(CameraRequest request)
        {
            CommandBuilder builder = new CommandBuilder().Request(request);

            if (!SavesImage(request)) return builder.Build();

            builder.AddParam("id", ImageSetName);

            return builder.Build();
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

#if DEBUG
        public void ClearSockets()
        {
            byte[] ignore = new byte[300];
            int total = 0;
            while (config.DataSocket.Available > 0)
            {
                total += config.DataSocket.Receive(ignore);
            }
            Console.WriteLine("Total bytes flushed: " + total);
        }
#endif

        public void ShutDown()
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

    }
}
