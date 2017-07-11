using Hub.Util;
using SharedDeviceItems;

namespace Hub.Helpers.Interface
{
    /// <summary>
    /// Implments a amanger for cameras so that image events can be syncrenized accross multiple cameras of differing types (local and remote)
    /// </summary>
    public interface ICameraManager
    {
        string SavePath { get; set; }
        int ProjectId { get; }
        ProjectMapper ProjectData { get; }

        void CaptureImageSet();
        void CaptureImageSet(CameraRequest wanted);

#if DEBUG
        /// <summary>
        /// use when debugging - clears every socket buffer of data
        /// </summary>
        void ClearSockets();
#endif
    }
}
