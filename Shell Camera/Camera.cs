using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharedDeviceItems.Exceptions;
using SharedDeviceItems.Interface;

// ReSharper disable ParameterHidesMember

namespace Shell_Camera
{
    public class ShellCamera : ICamera
    {
        private static int iLimit = 2000;

        private int resX = 1920, resY = 1080;
        private string name;
        private string location;
        private string currentDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

        public ShellCamera(string name, string saveLocation = "/scanImage/")
        {
            this.name = name;
            location = saveLocation;
        }

        public string CaptureImage(string identifier)
        {
            Console.WriteLine("--- Shell Capture ---");
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/raspistill",
                    Arguments = "-o " + name + identifier + ".jpg -w " + resX + " -h " + resY + " -q 100 -t 100",
                    UseShellExecute = false
                };
                Process proc = new Process { StartInfo = startInfo };

                proc.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Image capture failed: " + e.Message);
                throw new CaptureFailedException();
            }

            string loc = currentDir + name + identifier + ".jpg";
            int i = 0;
            Console.Write("Checking for image file");

            do
            {
                Thread.Sleep(10);
                ++i;
            } while (!File.Exists(loc) || i > iLimit);
            if (i > iLimit)
            {
                throw new CaptureFailedException("Image could not be found after taking the image");
            }
            Console.WriteLine(" ...Done");

            Console.Write("Checking if file is still writing");
            i = 0;
            int identicleCount = 0;
            bool b = true;
            long lastSize = -1;

            do
            {
                try
                {
                    if (File.GetAttributes(loc).HasFlag(FileAttributes.ReadOnly)) b = true;

                    //figure out if more data is being written to the file
                    FileInfo info = new FileInfo(loc);
                    if (info.Length <= 0) b = true;
                    else
                    {
                        if(info.Length > lastSize)
                        {
                            lastSize = info.Length;
                            identicleCount = 0;
                        }
                        else if(info.Length == lastSize)
                        {
                            ++identicleCount;
                            if(identicleCount > 2) b = false;
                        }
                    }

                    if (b) ++i;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Attempt " + i + " Failed: " + e.Message);
                    b = true;
                    ++i;
                }
            } while (b || i > iLimit / 2);
            Console.WriteLine(i > iLimit ? " ...Checking aborted" : " ...Done");

            Thread.Sleep(1000);

            return loc;
        }

        public void SetDirectory(string location)
        {
            this.location = location;
        }

        public void SetCameraName(string name)
        {
            this.name = name;
        }

        public void SetResolution(int x, int y)
        {
            resX = x;
            resY = y;
        }
    }
}
