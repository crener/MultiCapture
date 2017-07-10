using System.Text;
using Camera_Server;
using SharedDeviceItems.Interface;

namespace Camera_ServerTests.Mocks
{
    class MockRequestProcess : RequestProcess
    {
        public byte[] IncomingRequest { get; set; }
        public byte[] RequestResponse { get; set; }


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
            IncomingRequest = message;
            return RequestResponse ?? base.ProcessRequest(message);
        }

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
