using SharedDeviceItems;

namespace Hub.Helpers
{
    static class CameraHelper
    {
        /// <summary>
        /// checks if the image request needs to have a place to store images
        /// </summary>
        /// <param name="image">type of image request</param>
        /// <returns>true if save location is required</returns>
        public static bool SavesImage(CameraRequest image)
        {
            if (image == CameraRequest.SendFullResImage)
            {
                return true;
            }
            return false;
        }
    }
}
