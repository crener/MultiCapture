using System.Threading.Tasks;
using SharedDeviceItems;

namespace Hub.Threaded
{
    internal interface ICameraTask
    {
        Task ProcessRequest(CameraRequest request);
        void ShutDown();

        string ImageSetName { get; set; }
        string SavePath { get; set; }
#if DEBUG
        void ClearSockets();
#endif
    }
}
