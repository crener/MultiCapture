using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hub.Networking;
using SharedDeviceItems;
using SharedDeviceItems.Helpers;
using SharedDeviceItems.Interface;
using Shell_Camera;

namespace Camera_Server
{
    public class RequestProcess
    {
        protected ICamera camera = new ShellCamera("0");
        private ISocket client;
        private static Dictionary<string, CameraRequest> requestLookup = new Dictionary<string, CameraRequest>();
        protected string imageName = "0";

        public RequestProcess(ISocket client)
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

            if (requestMessage.Parameters.ContainsKey("id"))
            {
                imageName = requestMessage.Parameters["id"];
                Console.WriteLine("ImageName: " + imageName);
            }

            if (requestMessage.Request == CameraRequest.SetProporties)
            {
                if(requestMessage.Parameters.ContainsKey("name"))
                {
                    CameraSettings.AddSetting("name", requestMessage.Parameters["name"]);
                    camera.SetCameraName(requestMessage.Parameters["name"]);
                }
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
                    SendResponse(client, messageData, false);

                    if (File.Exists(imageLocation)) File.Delete(imageLocation);
                    return;
                case CameraRequest.SendTestImage:
                    //For testing, send a static image saved on the device
                    messageData = ByteHelpers.FileToBytes(Constants.DefualtHubSaveLocation() + "test.jpg");
                    SendResponse(client, messageData, false);
                    return;
                case CameraRequest.SetProporties:
                    AliveRequest(client);
                    return;
                default:
                    Console.WriteLine("Request Processing failed");
                    Console.WriteLine("\tRequest Name: " + request);
                    break;
            }

            FailReply(client);
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
        /// wrapper for sending bytes to get cleaner code 
        /// </summary>
        /// <param name="client">Socket that data will be sent too</param>
        /// <param name="response">The data that will be sent</param>
        /// <param name="needsEnd">true if end must be added to response</param>
        private static void SendResponse(ISocket client, byte[] response, bool needsEnd = true)
        {
            Console.WriteLine("Data size: " + response.Length);
            client.Send(EndOfMessage(Encoding.ASCII.GetBytes(response.Length.ToString())));
            client.Send(needsEnd ? EndOfMessage(response) : response);
        }

        /// <summary>
        /// wrapper for the alive request to be handled
        /// </summary>
        /// <param name="client"></param>
        private static void AliveRequest(ISocket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes(Constants.SuccessString + Constants.EndOfMessage);
            client.Send(msg);
        }

        private static void FailReply(ISocket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes(Constants.FailString + Constants.EndOfMessage);
            client.Send(msg);
        }
    }
}