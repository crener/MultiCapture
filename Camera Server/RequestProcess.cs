using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Camera;
using SharedDeviceItems;
using SharedDeviceItems.Interface;

namespace Camera_Server
{
    class RequestProcess
    {
        private ICamera camera = new ShellCamera("0");
        private Socket client;
        private static Dictionary<string, CameraRequest> requestLookup = new Dictionary<string, CameraRequest>()
        {
            {"0", CameraRequest.Alive},
            {"1", CameraRequest.SendFullResImage},
            {"9", CameraRequest.SendTestImage}
        };

        public RequestProcess(Socket client)
        {
            this.client = client;
        }

        public void ProcessRequest(string message)
        {
            CameraRequest request;
            if (!requestLookup.TryGetValue(message, out request))
            {
                SendResponse(client, EndOfMessage(FailedRequest()));
                return;
            }
            if (request == CameraRequest.Alive)
            {
                AliveRequest(client);
                return;
            }

            Console.WriteLine("Executing request: " + request);
            byte[] messageData = null;

            switch (request)
            {
                case CameraRequest.SendFullResImage:
                    string imageLocation = camera.CaptureImage("0");

                    messageData = FileToBytes(imageLocation);
                    SendResponse(client, EndOfMessage(messageData));

                    File.Delete(imageLocation);
                    return;
                case CameraRequest.SendTestImage:
                    //For testing, send a static image saved on the device
                    messageData = FileToBytes(Path.AltDirectorySeparatorChar + "scanimage" + Path.AltDirectorySeparatorChar + "test.jpg");
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

        private static byte[] FileToBytes(string location)
        {
            string fileName = location.Substring(location.LastIndexOf(Path.AltDirectorySeparatorChar));
            byte[] name = Encoding.ASCII.GetBytes(fileName + Constants.MessageSeperator),
                        file = File.ReadAllBytes(location),
                        eom = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            var messageData = new byte[name.Length + file.Length + eom.Length];
            name.CopyTo(messageData, 0);
            file.CopyTo(messageData, name.Length);
            eom.CopyTo(messageData, name.Length + file.Length);
            return messageData;
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
            byte[] msg = Encoding.ASCII.GetBytes("Cammera Active" + Constants.EndOfMessage);
            client.Send(msg);
        }
    }
}
