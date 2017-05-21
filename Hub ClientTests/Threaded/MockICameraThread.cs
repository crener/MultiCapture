using System;
using SharedDeviceItems;

namespace Hub.Threaded
{
    class MockICameraThread : ICameraThread
    {
        public bool wasStarted = false;

        public string ImageSetName { get; set; }
        public bool Finish { get; set; }
        public string SavePath { get; set; }
        public CameraRequest Request { get; set; }

        public void Start()
        {
            wasStarted = true;
        }

#if DEBUG
        public void ClearSockets()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
