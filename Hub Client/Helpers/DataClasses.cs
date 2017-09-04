using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hub.Networking;
using Hub.ResponseSystem;
using SharedDeviceItems;
using SharedDeviceItems.Exceptions;
using SharedDeviceItems.Networking.CameraHubConnection;

namespace Hub.Helpers
{
    /// <summary>
    /// Helps to setup a camera and makes sure that it is able to connect and recive data back from the camera before
    /// passing it on to mission critical systems.
    /// </summary>
    public class CameraSocket
    {
        public ISocket DataSocket { get; set; }
        public CameraConfiguration Config { get; set; }

        public bool Setup()
        {
            if (Config.Address == 0) throw new InvalidOperationException("Configuration address not configured");
            if (Config.Port == 0) throw new InvalidOperationException("Configuration port not configured");

            //check using ping if the camera is using http
            if(Config.useHttpClient)
            {
                try
                {
                    IRequester request = new HttpCameraRequester(Config.Address, Config.Port);
                    byte[] response = request.Request(CameraRequest.Alive);

                    if (response.Length <= 0)
                    {
                        Console.WriteLine("Camera not active, No data received");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Camera response = {0}", Encoding.ASCII.GetString(response));
                        return true;
                    }
                }
                catch(CaptureFailedException)
                {
                    Console.WriteLine("No response from camera");
                    return false;
                }
            }

            try
            {
                //check if the camera is active
                DataSocket.Connect(new IPEndPoint(Config.Address, Config.Port));
                IRequester requester = new ChunkRequester(DataSocket);
                byte[] bytesRec = requester.Request(CameraRequest.Alive);

                if (bytesRec.Length <= 0)
                {
                    Console.WriteLine("Camera not active, No data received");
                    DataSocket.Shutdown(SocketShutdown.Both);
                    DataSocket.Close();
                    return false;
                }
                else
                {
                    Console.WriteLine("Camera response = {0}", Encoding.ASCII.GetString(bytesRec));
                }
            }
#if DEBUG
            catch (SocketException e)
#else
            catch (SocketException e)
#endif
            {
#if DEBUG
                Console.WriteLine("Socket Exception : {0}", e);
#endif

                if (!DataSocket.Connected) return false;

                DataSocket.Shutdown(SocketShutdown.Both);
                DataSocket.Close();
                return false;
            }

            return true;
        }
    }
}