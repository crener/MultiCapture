using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hub.SaveLoad;
using SharedDeviceItems;

namespace Hub.Helpers
{
    public class CameraSocket
    {
        public Socket DataSocket { get; set; }
        public CameraConfiguration Config { get; set; }

        public bool Setup()
        {
            try
            {
                //check if the camera is active
                DataSocket.Connect(new IPEndPoint(Config.Address, Config.Port));
                DataSocket.Send(Encoding.ASCII.GetBytes((int)CameraRequest.Alive + Constants.EndOfMessage));

                //grab data
                byte[] recieveData = new byte[1000];
                int bytesRec = DataSocket.Receive(recieveData);
                Console.WriteLine("Camera response = {0}", Encoding.ASCII.GetString(recieveData, 0, bytesRec - Constants.EndOfMessage.Length));

                //if there was no data the camera must have been off
                if (bytesRec <= 0)
                {
                    Console.WriteLine("Camera not active, No data recieved");
                    DataSocket.Shutdown(SocketShutdown.Both);
                    DataSocket.Close();
                    return false;
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Exception : {0}", e);

                if(!DataSocket.Connected) return false;

                DataSocket.Shutdown(SocketShutdown.Both);
                DataSocket.Close();
                return false;
            }

            return true;
        }
    }
}