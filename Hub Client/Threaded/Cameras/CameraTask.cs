using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub.Util;
using SharedDeviceItems;
using SharedDeviceItems.Networking.CameraHubConnection;
using static Hub.Helpers.CameraHelper;

namespace Hub.Threaded
{
    class CameraTask : ICameraTask
    {
        public string SavePath { get; set; }
        private CameraSocket config;
        private IRequester connection;

        public CameraTask(CameraSocket socket, string saveLocation)
        {
            config = socket;
            socket.DataSocket.ReceiveBufferSize = Constants.CameraBufferSize * 2;
            SavePath = saveLocation;

            if(socket.Config.useHttpClient)
                connection = new HttpCameraRequester(socket.Config.Address, socket.Config.Port);
            else
            {
                connection = new ChunkRequester(socket.DataSocket);
                connection.ClearSocket();
            }
        }

        public Task ProcessRequest(CameraRequest request)
        {
            return new Task(() => InternalRequestProcess(request));
        }

        //todo figure out how to tell the manager that the current camera has disconnected so that it will no longer be queried for responses
        private void InternalRequestProcess(CameraRequest request)
        {
            //start asking the camera for a new image
            byte[] data;

            try
            {
                data = connection.Request(BuildExternalCommand(request));
            }
            catch(InvalidDataException)
            {
                //The data from the socket is incorrect.
                int bytes = connection.ClearSocket();
                Console.WriteLine("Camera " + config.Config.Id + " failed sending its image");
#if DEBUG
                Console.WriteLine("\tFlushed " + bytes + " of data"); 
#endif
                return;
            }

            //extract image data
            string imageName;
            byte[] imageData;
            ByteManipulation.SeparateData(out imageName, data, out imageData);
            if (imageName == "" || imageData.Length <= 0)
            {
                Console.WriteLine("No Image data recieved!!");
                Console.WriteLine("Debug data:");
                Console.WriteLine("\tThread Camera: " + config.Config.Id);
                Console.WriteLine("\tImage set id: " + GenericManager.ImagesetId);
                Console.WriteLine("\tImage return string: " + Encoding.ASCII.GetString(data));
                return;
            }

            SaveData(imageData, SavePath + Path.DirectorySeparatorChar + imageName);
            Deployer.CurrentProject.AddImage(GenericManager.ImagesetId, imageName, config.Config.Id);

            Console.WriteLine("Camera " + config.Config.Id + " image saved");
        }

        private byte[] BuildExternalCommand(CameraRequest request)
        {
            CommandBuilder builder = new CommandBuilder(request);

            if (!SavesImage(request)) return builder.Build();

            builder.AddParam("id", GenericManager.ImagesetId.ToString());

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

        public void Dispose()
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
