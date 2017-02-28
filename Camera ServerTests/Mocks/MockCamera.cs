using System.IO;
using SharedDeviceItems.Interface;
using SharedDeviceItems.Helpers;

namespace Camera_ServerTests.Mocks
{
    class MockCamera : ICamera
    {
        public string directory { get; set; }
        public string name { get; set; }
        public string identifier { get; set; }
        public byte[] cameraData { get; set; }
        public int imageX { get; set; }
        public int imageY { get; set; }
        public string lastPath { get; set; }

        public void SetDirectory(string location)
        {
            directory = location;
        }

        public void SetCameraName(string name)
        {
            this.name = name;
        }

        public string CaptureImage(string identifier)
        {
            this.identifier = identifier;
            string path = directory + name + this.identifier + ".jpg";

            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                writer.Write(cameraData, 0, cameraData.Length);
            }

            lastPath = path;
            return path;
        }

        public void SetResolution(int x, int y)
        {
            imageX = x;
            imageY = y;
        }

        public byte[] CaptureImageByte(string identifier)
        {
            string imageLocation = CaptureImage(identifier);
            byte[] data = ByteHelpers.FileToBytes(imageLocation);
            if (File.Exists(imageLocation)) File.Delete(imageLocation);
            return data;
        }
    }
}
