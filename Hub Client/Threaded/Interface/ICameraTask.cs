using System;
using System.Threading.Tasks;
using SharedDeviceItems;

namespace Hub.Threaded
{
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
