using System.Dynamic;
using Camera_Server;
using Hub.Networking;
using SharedDeviceItems.Interface;

namespace Camera_ServerTests.Mocks
{
    class MockRequestProcess : RequestProcess
    {
        public MockRequestProcess(ISocket client) : base(client) { }

        public MockRequestProcess(ISocket client, ICamera camera) : base(client)
        {
            base.camera = camera;
        }

        public MockRequestProcess(ISocket client, bool ignore) : base(client)
        {
            base.camera = new IgnoreCamera();
        }

        public override void ProcessRequest(byte[] message)
        {
            if (message.Length > 0) RequestProcessData = message;
            base.ProcessRequest(message);
        }


        public byte[] RequestProcessData { get; set; }
        public string GetImageName() { return imageName; }
        public void SetImageName(string name) { imageName = name; }

        private class IgnoreCamera : ICamera
        {
            public void SetDirectory(string location)
            {

            }

            public void SetCameraName(string name)
            {

            }

            public string CaptureImage(string identifier)
            {
                return "";
            }

            public void SetResolution(int x, int y)
            {

            }
        }
    }
}
