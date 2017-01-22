using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hub.Networking;
using SharedDeviceItems;

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

            try
            {
                //check if the camera is active
                DataSocket.Connect(new IPEndPoint(Config.Address, Config.Port));
                DataSocket.Send(Encoding.ASCII.GetBytes((int)CameraRequest.Alive + Constants.EndOfMessage));

                //grab data
                byte[] recieveData = new byte[1000];
                int bytesRec = DataSocket.Receive(recieveData);
                //if there was no data the camera must have been off
                if(bytesRec <= 0)
                {
                    Console.WriteLine("Camera not active, No data recieved");
                    DataSocket.Shutdown(SocketShutdown.Both);
                    DataSocket.Close();
                    return false;
                }
                else
                {
                    Console.WriteLine("Camera response = {0}",
                        Encoding.ASCII.GetString(recieveData, 0, bytesRec - Constants.EndOfMessage.Length));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Exception : {0}", e);

                if (!DataSocket.Connected) return false;

                DataSocket.Shutdown(SocketShutdown.Both);
                DataSocket.Close();
                return false;
            }

            return true;
        }
    }
}