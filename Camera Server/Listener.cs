using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hub.Helpers.Wrapper;
using Hub.Networking;
using SharedDeviceItems;
using SharedDeviceItems.Helpers;

namespace Camera_Server
{
    public class Listener
    {
        private static string data;
        protected bool stop { get; set; }
        protected ISocket listener;

        public void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[Constants.CameraBufferSize];
            stop = false;

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = NetworkHelpers.GrabIpv4(ipHostInfo);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, int.Parse(CameraSettings.GetSetting("port", "11003")));

            Console.WriteLine("IP address = " + ipAddress);
#pragma warning disable CS0618 // Type or member is obsolete
            Console.WriteLine("IP long address = " + ipAddress.Address);
#pragma warning restore CS0618 // Type or member is obsolete

            // Create a TCP/IP socket.
            if(listener == null ) listener = 
                    new WSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ISocket handler = null;

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (!stop)
                {
                    try
                    {
                        Console.WriteLine("Waiting for a connection...");
                        // Thread is suspended while waiting for an incoming connection.
                        handler = listener.Accept();
                        data = null;
                        Console.WriteLine("Connected!!");

                        while (Connected(handler) && !stop)
                        {
                            RequestProcess process = NewProcessor(handler);
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
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception thrown");
                        Console.WriteLine("\tmessage: " + e.Message);
#if DEBUG
                        Console.WriteLine("\tstack trace:" + e.StackTrace);
#endif
                        if(data.Length > 0) Console.WriteLine("\tlast request data: " + data);
                        else Console.WriteLine("\tlast request data: <Empty string>");
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
        private bool Connected(ISocket s)
        {
            if (s.Connected && s.Poll(1000, SelectMode.SelectRead) && s.Available == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Layer of abstarction for creating a request process so that tests can pass in
        /// a slightly more open version of the handeler
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected virtual RequestProcess NewProcessor(ISocket handler)
        {
            return new RequestProcess(handler);
        }
    }
}
