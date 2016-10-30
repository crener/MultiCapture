using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharedDeviceItems.Interface;
using SharedDeviceItems.Exceptions;
// ReSharper disable ParameterHidesMember

namespace Camera
{
    public class ShellCamera : ICamera
    {
        private int resX = 1920, resY = 1080;
        private string name;
        private string location;
        private string currentDir = System.IO.Directory.GetCurrentDirectory();

        public ShellCamera(string name, string saveLocation = "/scanImage/")
        {
            this.name = name;
            location = saveLocation;
        }

        public string CaptureImage(string identifier)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/raspistill",
                Arguments = "-o " + name + identifier + ".jpg -w " + resX + " -h " + resY + " -q 100 -t 10",
                UseShellExecute = false
            };
            Process proc = new Process { StartInfo = startInfo };

            try
            {
                proc.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Image capture failed: " + e.Message);
                throw new CaptureFailedException();
            }

            int i = 0;
            do
            {
                Thread.Sleep(10);
                ++i;
            } while (File.Exists(currentDir + name + identifier + ".jpg") || i > 1000);

            return currentDir + name + identifier + ".jpg";
        }

        public void SetDirectory(string location)
        {
            this.location = location;
        }

        public void SetCameraName(string name)
        {
            this.name = name;
        }

        public void SetResulution(int x, int y)
        {
            resX = x;
            resY = y;
        }
    }
}
