using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SharedDeviceItems;
using SharedDeviceItems.Helpers;

namespace Camera_Server
{
    public class Listener
    {
        private static string data;

        public void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[Constants.CameraBufferSize];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the host running the application.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = NetworkHelpers.GrabIpv4(ipHostInfo);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Int32.Parse(CameraSettings.GetSetting("port")));

            Console.WriteLine("IP address = " + ipAddress);
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
                    Console.WriteLine("Connected!!");

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        RequestProcess process = new RequestProcess(handler);
                        bytes = new byte[Constants.CameraBufferSize];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf(Constants.EndOfMessage) > -1)
                        {
                            //process data
                            process.ProcessRequest(bytes);

                            // Show the data on the console.
                            Console.WriteLine("Data received : {0}", data);
                            data = "";
                        }
                        Console.WriteLine("Waiting for next request...");

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

        /// <summary>
        /// test if the socket is connected to a device
        /// </summary>
        /// <param name="s">socket that needs to be tested</param>
        /// <returns>true if connected to a client</returns>
        private bool Connected(Socket s)
        {
            if (s.Connected && s.Poll(1000, SelectMode.SelectRead) && s.Available == 0)
                return false;

            return true;
        }
    }
}
