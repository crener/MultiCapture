using System.Text;
using Camera_Server;
using Hub.Networking;
using SharedDeviceItems.Interface;

namespace Camera_ServerTests.Mocks
{
    class MockRequestProcess : RequestProcess
    {
        public MockRequestProcess(ICamera client) : base(client) { }

        public MockRequestProcess(ICamera client, ICamera camera) : base(client)
        {
            base.camera = camera;
        }

        public MockRequestProcess(ICamera client, bool ignore) : base(client)
        {
            base.camera = new IgnoreCamera();
        }

        public override byte[] ProcessRequest(byte[] message)
        {
            if (message.Length > 0) RequestProcessData = message;
            return base.ProcessRequest(message);
        }


        public byte[] RequestProcessData { get; set; }

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

            public byte[] CaptureImageByte(string identifier)
            {
                return Encoding.ASCII.GetBytes("");
            }
        }
    }
}
