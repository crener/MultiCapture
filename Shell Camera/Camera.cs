﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharedDeviceItems;
using SharedDeviceItems.Exceptions;
using SharedDeviceItems.Interface;
using SharedDeviceItems.Helpers;

// ReSharper disable ParameterHidesMember

namespace Shell_Camera
{
    public class ShellCamera : ICamera
    {
        private static int iLimit = 2000;

        private int resX = 1920, resY = 1080;
        private string name;
        private string location;
        private Rotation rotation = Rotation.Zero;
        private bool vFlip = false, hFlip = false;
        private string currentDir;

        public ShellCamera(string name, string saveLocation = "/scanimage/")
        {
            this.name = name;
            location = saveLocation;

            try
            {
                currentDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
            }
            catch(FileNotFoundException)
            {
                currentDir = Path.GetTempPath();
            }
        }

        public string CaptureImage(string identifier)
        {
            Console.WriteLine("--- Shell Capture ---");
            string loc = currentDir + name + identifier + ".jpg";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/raspistill",
                    Arguments = "-o " + loc + " -w " + resX + " -h " + resY + " -q 100 -t 100" +
                        (rotation == Rotation.Zero ? "" : " -rot " + (int)rotation) + (vFlip ? " -vf" : "") + (hFlip ? " -hf" : ""),
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

            int i = 0;

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

        public byte[] CaptureImageByte(string identifier)
        {
            string imageLocation = CaptureImage(identifier);
            byte[] data = ByteHelpers.FileToBytes(imageLocation);
            if (File.Exists(imageLocation)) File.Delete(imageLocation);
            return data;
        }

        public void setFlip(bool verticleFlip, bool horizontalFlip)
        {
            vFlip = verticleFlip;
            hFlip = horizontalFlip;
        }

        public void setRotation(Rotation rotation)
        {
            this.rotation = rotation;
        }
    }
}
