using System.IO;

namespace SharedDeviceItems
{
    public enum CameraRequest
    {
        Alive = 0,
        SendFullResImage = 1,
        SendTestImage = 9,
        SetProporties = 8
    }

    public static class Constants
    {
        #region data transfer settings
        public const string EndOfMessage = "xtx";
        public const string MessageSeperator = "<,,,>";
        public const string FailString = "FAIL";
        public const string SuccessString = "SUCCESS";

        public const int ByteArraySize = 10485760;
        public const int CameraBufferSize = 1048576;

        public const string ParamSeperator = "&&";
        public const string ParamKeyValueSeperator = "=";
        #endregion

        #region Project file settings
        public const string ProjectFileExtention = "XML";
        #endregion

        public static string DefualtHubSaveLocation()
        {
            return Path.DirectorySeparatorChar + "scanimage";
        }
    }
}
