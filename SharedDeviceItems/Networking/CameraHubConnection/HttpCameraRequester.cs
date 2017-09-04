using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharedDeviceItems.Exceptions;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public class HttpCameraRequester : IRequester
    {
        private readonly string savePath;
        private string basicAddress;
        private readonly ProcessStartInfo basicStart;

        public HttpCameraRequester(long address, int port)
        {
            savePath = "/tmp/response" + port;
            basicAddress = "http://" + new IPEndPoint(address, port).Address + ":" + port + "/";

            basicStart = new ProcessStartInfo
            {
                FileName = "/usr/bin/wget",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
        }

        public byte[] Request(CameraRequest request)
        {
            return Request(Encoding.ASCII.GetBytes(((int)request).ToString()));
        }

        public byte[] Request(byte[] requestData)
        {
            string url = basicAddress + Encoding.ASCII.GetString(requestData);

            //get and store the file in temp storage
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = basicStart;
                startInfo.Arguments = url + " -O " + savePath;
                process.StartInfo = startInfo;

                process.Start();
                process.WaitForExit(8000);
            }
            catch (Exception e)
            {
                Console.WriteLine("Request Failed: " + e.Message);
#if DEBUG
                Console.WriteLine(e);
#endif

                throw new CaptureFailedException("Data transfer failed");
            }

            //load and return the request data
            if (!File.Exists(savePath))
                throw new CaptureFailedException("No data file was found");

            try
            {
                return File.ReadAllBytes(savePath);
            }
            finally
            {
                try
                {
                    File.Delete(savePath);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public int ClearSocket()
        {
            return 0;
        }
    }
}
