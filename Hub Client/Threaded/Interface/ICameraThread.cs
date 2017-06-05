using SharedDeviceItems;

namespace Hub.Threaded
{
    public interface ICameraThread
    {
        string ImageSetName { get; set; }
        bool Finish { get; set; }
        string SavePath { get; set; }
        CameraRequest Request { get; set; }

#if DEBUG
        void ClearSockets();
#endif
        void Start();
    }
}