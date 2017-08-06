using System;
using System.IO;
using SharedDeviceItems;
using SharedDeviceItems.Interface;
using SharedDeviceItems.Helpers;

namespace CameraServerTests.Mocks
{
    class MockCamera : ICamera
    {
        public string Directory { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public byte[] CameraData { get; set; }
        public int ImageX { get; set; }
        public int ImageY { get; set; }
        public string LastPath { get; set; }


        public void SetDirectory(string location)
        {
            Directory = location;
        }

        public void SetCameraName(string name)
        {
            this.Name = name;
        }

        public string CaptureImage(string identifier)
        {
            throw new NotImplementedException();

            /*this.Identifier = identifier;
            string path = Directory + Name + this.Identifier + ".jpg";

            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                writer.Write(CameraData, 0, CameraData.Length);
            }

            LastPath = path;
            return path;*/
        }

        public void SetResolution(int x, int y)
        {
            ImageX = x;
            ImageY = y;
        }

        public byte[] CaptureImageByte(string identifier)
        {
            return CameraData;
        }

        public void setFlip(bool verticleFlip, bool horizontalFlip)
        {
        }

        public void setRotation(Rotation rotation)
        {
        }
    }
}
