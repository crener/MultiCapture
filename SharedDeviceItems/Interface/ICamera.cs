namespace SharedDeviceItems.Interface
{
    public interface ICamera
    {
        /// <summary>
        /// The location that the camera will store images
        /// </summary>
        /// <param name="location">the folder that the image will be sored</param>
        void SetDirectory(string location);

        /// <summary>
        /// The start of the name of the image that will be saved to disk
        /// </summary>
        /// <param name="name">base name of the image file</param>
        void SetCameraName(string name);

        /// <summary>
        /// capture picture using xy parameters and save with identifier added to image name in directory
        /// </summary>
        /// <param name="identifier">name added to the end of the standard image name</param>
        /// <returns>location of the saved image</returns>
        string CaptureImage(string identifier);

        /// <summary>
        /// set the resolution of the camera
        /// </summary>
        /// <param name="x">X-axis reulution</param>
        /// <param name="y">Y-axis reulution</param>
        void SetResolution(int x, int y);
    }
}
