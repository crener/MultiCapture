using SharedDeviceItems;

namespace Hub.Helpers.Interface
{
    public interface ICameraManager
    {
        string SavePath { get; set; }
        int ProjectId { get; }

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
