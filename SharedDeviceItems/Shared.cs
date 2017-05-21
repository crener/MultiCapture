using System.IO;

namespace SharedDeviceItems
{
    public enum CameraRequest
    {
        Unknown = -1,
        Alive = 0,
        SendFullResImage = 1,
        SendTestImage = 9,
        SetProporties = 8
    }

    public static class Constants
    {
        #region data transfer settings
        public const string EndOfMessage = "xtx";
        public const string MessageSeparator = "<,,,>";
        public const string FailString = "FAIL";
        public const string SuccessString = "SUCCESS";

        public const int ByteArraySize = 10485760;
        public const int CameraBufferSize = 128;

        public const string ParamSeparator = "&&";
        public const string ParamKeyValueSeparator = "=";
        #endregion

        #region Project file settings
        public const string ProjectFileExtention = "XML";
        #endregion

        public static string DefaultHubSaveLocation()
        {
            return Path.DirectorySeparatorChar + "scanimage" + Path.DirectorySeparatorChar;
        }
    }
}
