using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Python_Shell_Camera;
using SharedDeviceItems;
using SharedDeviceItems.Helpers;
using SharedDeviceItems.Interface;
using Shell_Camera;

namespace Camera_Server
{
    class RequestProcess
    {
        //private ICamera camera = new ShellCamera("0");
        private ICamera camera = new PythonShellCamera();
        private Socket client;
        private static Dictionary<string, CameraRequest> requestLookup = new Dictionary<string, CameraRequest>();

        public RequestProcess(Socket client)
        {
            this.client = client;

            if (requestLookup.Count <= 0)
            {
                Console.WriteLine("request initialisation");
                CameraRequest[] enums = (CameraRequest[])Enum.GetValues(typeof(CameraRequest));
                foreach (CameraRequest value in enums)
                {
                    Console.WriteLine("Key = " + "" + (int)value + ", value = " + value);
                    requestLookup.Add("" + (int)value, value);
                }
            }
        }

        public void ProcessRequest(string message)
        {
            CameraRequest request;
            if (!requestLookup.TryGetValue(message, out request))
            {
                Console.WriteLine("404 - Request not found!!");
                SendResponse(client, EndOfMessage(FailedRequest()));
                return;
            }
            if (request == CameraRequest.Alive)
            {
                AliveRequest(client);
                return;
            }

            Console.WriteLine("Executing request: " + request);
            byte[] messageData;

            switch (request)
            {
                case CameraRequest.SendFullResImage:
                    string imageLocation = camera.CaptureImage("0");

                    messageData = ByteHelpers.FileToBytes(imageLocation);
                    SendResponse(client, EndOfMessage(messageData));

                    if (File.Exists(imageLocation)) File.Delete(imageLocation);
                    return;
                case CameraRequest.SendTestImage:
                    //For testing, send a static image saved on the device
                    messageData = ByteHelpers.FileToBytes(Path.DirectorySeparatorChar + "scanimage" + Path.DirectorySeparatorChar + "test.jpg");
                    SendResponse(client, EndOfMessage(messageData));
                    return;
                default:
                    messageData = FailedRequest();
                    break;
            }

            SendResponse(client, EndOfMessage(messageData));
        }

        /// <summary>
        /// quick method for adding end of message string to the end of a data transmission
        /// </summary>
        /// <param name="data">the data message</param>
        private static byte[] EndOfMessage(byte[] data)
        {
            byte[] message = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] outData = new byte[data.Length + message.Length];

            data.CopyTo(outData, 0);
            message.CopyTo(outData, data.Length);

            return outData;
        }

        /// <summary>
        /// wrapper for the standard failed response 
        /// </summary>
        private static byte[] FailedRequest()
        {
            return Encoding.ASCII.GetBytes(Constants.FailString + Constants.EndOfMessage);
        }

        /// <summary>
        /// wrapper for sending bytes to get cleaner code 
        /// </summary>
        /// <param name="client">Socket that data will be sent too</param>
        /// <param name="reponse">The data that will be sent</param>
        private static void SendResponse(Socket client, Byte[] reponse)
        {
            client.Send(reponse);
        }

        /// <summary>
        /// wrapper for the alive request to be handled
        /// </summary>
        /// <param name="client"></param>
        private static void AliveRequest(Socket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes("Camera Active" + Constants.EndOfMessage);
            client.Send(msg);
        }
    }
}
