using System;
using System.Threading.Tasks;
using SharedDeviceItems;

namespace Hub.Threaded
{
    /// <summary>
    /// Manager for an individule camera to manage data and state. Uses raw tasks to achieve multithreading
    /// </summary>
    internal interface ICameraTask : IDisposable
    {
        Task ProcessRequest(CameraRequest request);
        string ImageSetName { get; set; }
        string SavePath { get; set; }

#if DEBUG
        void ClearSockets();
#endif
    }
}
