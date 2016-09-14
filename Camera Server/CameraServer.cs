using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.IO;
using SharedDeviceItems;

namespace Camera
{
    public static class SynchronousSocketListener
    {
        private const int bufferSize = 1048576;

        // Incoming data from the client.
        private static string data = null;

        private static Dictionary<string, CameraRequest> requestLookup = new Dictionary<string, CameraRequest>()
        {
            {"0", CameraRequest.Alive},
            {"1", CameraRequest.SendFullResImage},
            {"9", CameraRequest.SendTestImage}
        };

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[bufferSize];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GrabIpv4(ipHostInfo);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11003);

            Console.WriteLine("IP address = " + ipAddress.ToString());
#pragma warning disable CS0618 // Type or member is obsolete
            Console.WriteLine("IP long address = " + ipAddress.Address);
#pragma warning restore CS0618 // Type or member is obsolete

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket handler = null;

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        Console.WriteLine("Waiting for next request...");
                        bytes = new byte[bufferSize];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf(Constants.EndOfMessage) > -1)
                        {
                            //process data
                            data = data.Remove(data.Length - Constants.EndOfMessage.Length,
                                Constants.EndOfMessage.Length);
                            ProcessRequest(handler, data);

                            // Show the data on the console.
                            Console.WriteLine("Data received : {0}", data);
                            data = "";
                        }

                        if (!Connected(handler)) break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (handler != null)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }

        private static IPAddress GrabIpv4(IPHostEntry ipHostInfo)
        {
            foreach (IPAddress item in ipHostInfo.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    return item;
                }
            }
            return ipHostInfo.AddressList[0];
        }

        private static void ProcessRequest(Socket client, string message)
        {
            CameraRequest request;
            if (!requestLookup.TryGetValue(message, out request))
            {
                FailedRequest(client);
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
                    break;
                case CameraRequest.SendTestImage:
                    //For testing, send a static image saved on the device
                    byte[] name = Encoding.ASCII.GetBytes("test.jpg" + Constants.MessageSeperator),
                        file = File.ReadAllBytes(Path.AltDirectorySeparatorChar + "scanimage" +
                                              Path.AltDirectorySeparatorChar + "test.jpg"),
                        eom = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

                    messageData = new byte[name.Length + file.Length + eom.Length];
                    name.CopyTo(messageData, 0);
                    file.CopyTo(messageData, name.Length);
                    eom.CopyTo(messageData, name.Length + file.Length);

                    for (int i = 0; i < 12; i++)
                    {
                        Console.WriteLine("Data " + i + ": " + messageData[i]);
                    }
                    break;
                default:
                    FailedRequest(client);
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
        /// <param name="client"></param>
        private static void FailedRequest(Socket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes(Constants.FailString + Constants.EndOfMessage);
            client.Send(msg);
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

        /// <summary>
        /// wrapper for sending bytes to get cleaner code 
        /// </summary>
        /// <param name="client">Socket that data will be sent too</param>
        /// <param name="reponse">The data that will be sent</param>
        private static void SendResponse(Socket client, Byte[] reponse)
        {
            client.Send(reponse);
        }

        public static bool Connected(Socket s)
        {
            if (s.Connected && s.Poll(1000, SelectMode.SelectRead) && s.Available == 0)
                return false;

            return true;
        }
    }
}