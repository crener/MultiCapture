using SharedDeviceItems;

namespace Hub.Threaded
{
    /// <summary>
    /// Manager for an individule camera to manage data and state. Uses raw threads to achieve multithreading
    /// </summary>
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