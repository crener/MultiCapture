using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using CameraServer;
using SharedDeviceItems.Exceptions;
using SharedDeviceItems.Helpers;
using Shell_Camera;

namespace HttpCameraServer
{
    class HttpResponder
    {

        public void StartListening()
        {
            string ip, longIp;
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = NetworkHelpers.GrabIpv4(ipHostInfo);

                ip = ipAddress.ToString();
                longIp = ipAddress.Address.ToString();

                Console.WriteLine("Camera Name\t= " + CameraSettings.GetSetting("name"));
                Console.WriteLine("IP address\t= " + ipAddress);
                Console.WriteLine("IP long address\t= " + longIp);
                Console.WriteLine("Port\t\t= " + CameraSettings.GetSetting("port"));
                Console.WriteLine();
            }

            HttpListener server = new HttpListener();
            RequestProcess process = NewProcessor();

            //server.Prefixes.Add("http://localhost:" + CameraSettings.GetSetting("port") + "/");
            server.Prefixes.Add("http://" + ip + ":" + CameraSettings.GetSetting("port") + "/");

            server.Start();
            Console.WriteLine("Listening");

            try
            {
                while (server.IsListening)
                {
                    HttpListenerContext context = server.GetContext();
                    Console.WriteLine("New request received");

                    try
                    {
                        if(context.Request.RawUrl.Length <= 0)
                        {
                            Console.WriteLine("No command in the request header");
                            context.Response.StatusCode = 400;
                            continue;
                        }

                        string command = context.Request.RawUrl.Substring(1);
                        byte[] responseData = process.ProcessRequest(command);

                        context.Response.StatusCode = 200;
                        context.Response.ContentLength64 = responseData.Length;
                        context.Response.OutputStream.Write(responseData, 0, responseData.Length);
                    }
                    catch(KeyNotFoundException)
                    {
                        Console.WriteLine("No command in the request header");
                        context.Response.StatusCode = 400;
                    }
                    catch(CaptureFailedException)
                    {
                        context.Response.StatusCode = 500;
                    }
                    catch(Exception)
                    {
                        context.Response.StatusCode = 500;
                        throw;
                    }
                    finally
                    {
                        try
                        {
                            Console.WriteLine("Request Finished");
                            context.Response.OutputStream.Close();
                        }
                        catch(HttpListenerException)
                        {
                            Console.WriteLine("Connection error occurred before completing the request");
                        }
                    }
                }
            }
            finally
            {
                server.Stop();
            }
        }

        protected virtual RequestProcess NewProcessor()
        {
            return new RequestProcess(new ShellCamera("0"));
        }
    }
}
