using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using SharedDeviceItems;
using SharedDeviceItems.Helpers;
using SharedDeviceItems.Interface;
using Shell_Camera;

namespace Camera_Server
{
    class RequestProcess
    {
        private ICamera camera = new ShellCamera("0");
        private Socket client;
        private static Dictionary<string, CameraRequest> requestLookup = new Dictionary<string, CameraRequest>();
        private string imageName = "0";

        public RequestProcess(Socket client)
        {
            this.client = client;
            camera.SetCameraName(CameraSettings.GetSetting("name"));
            camera.SetResolution(3280, 2464);

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

        public void ProcessRequest(byte[] message)
        {
            CommandReader requestMessage = new CommandReader(message);
            ProcessRequest(requestMessage);
        }

        public void ProcessRequest(string message)
        {
            CommandReader requestMessage = new CommandReader(message);
            ProcessRequest(requestMessage);
        }

        private void ProcessRequest(CommandReader requestMessage)
        {
            if (requestMessage.Request == CameraRequest.Alive)
            {
                AliveRequest(client);
                return;
            }
            else if (requestMessage.Request == CameraRequest.SendTestImage)
            {
                InternalProcess(CameraRequest.SendTestImage);
                return;
            }

            imageName = requestMessage.Parameters["id"];
            Console.WriteLine("ImageName: " + imageName);

            if (requestMessage.Request == CameraRequest.SetProporties)
            {
                CameraSettings.AddSetting("name", requestMessage.Parameters["name"]);
                camera.SetCameraName(requestMessage.Parameters["name"]);
                AliveRequest(client);
                return;
            }

            InternalProcess(requestMessage.Request);
        }

        private void InternalProcess(CameraRequest request)
        {
            Console.WriteLine("Executing request: " + request);
            byte[] messageData;

            switch (request)
            {
                case CameraRequest.Alive:
                    AliveRequest(client);
                    return;
                case CameraRequest.SendFullResImage:
                    string imageLocation = camera.CaptureImage(imageName);

                    messageData = ByteHelpers.FileToBytes(imageLocation);
                    SendResponse(client, messageData);

                    if (File.Exists(imageLocation)) File.Delete(imageLocation);
                    return;
                case CameraRequest.SendTestImage:
                    //For testing, send a static image saved on the device
                    messageData = ByteHelpers.FileToBytes(Path.DirectorySeparatorChar + "scanimage" + Path.DirectorySeparatorChar + "test.jpg");
                    SendResponse(client, messageData);
                    return;
                case CameraRequest.SetProporties:
                    AliveRequest(client);
                    return;
                default:
                    Console.WriteLine("Request Processing failed");
                    Console.WriteLine("\tRequest Name: " + request);
                    messageData = FailedRequest();
                    break;
            }

            SendResponse(client, messageData);
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
        /// <param name="response">The data that will be sent</param>
        private static void SendResponse(Socket client, byte[] response)
        {
            Console.WriteLine("Data size: " + response.Length);
            client.Send(EndOfMessage(Encoding.ASCII.GetBytes(response.Length.ToString())));
            client.Send(EndOfMessage(response));
        }

        /// <summary>
        /// wrapper for the alive request to be handled
        /// </summary>
        /// <param name="client"></param>
        private static void AliveRequest(Socket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes(Constants.SuccessString + Constants.EndOfMessage);
            client.Send(msg);
        }

        private static void FailReply(Socket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes(Constants.FailString + Constants.EndOfMessage);
            client.Send(msg);
        }
    }
}