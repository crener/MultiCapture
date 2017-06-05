using System;
using System.Threading.Tasks;
using SharedDeviceItems;

namespace Hub.Threaded
{
    class MockICameraTask : ICameraTask
    {
        public bool ShutDownTriggered = false;
        public CameraRequest lastRequest = CameraRequest.Unknown;

        public Task ProcessRequest(CameraRequest request)
        {
            lastRequest = request;
            return new Task(() => Task.Delay(2));
        }

        public void Dispose()
        {
            ShutDownTriggered = true;
        }

        public string ImageSetName { get; set; }
        public string SavePath { get; set; }

#if DEBUG
        public void ClearSockets()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
