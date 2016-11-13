namespace SharedDeviceItems
{
    public enum CameraRequest
    {
        Alive = 0,
        SendFullResImage = 1,
        SendTestImage = 9
    }

    public static class Constants
    {
        public const string EndOfMessage = "xtx";
        public const string MessageSeperator = "<,,,>";
        public const string FailString = "FAIL";
        public const int ByteArraySize = 10485760;
        public const int CameraBufferSize = 1048576;

        public const string ParamSeperator = "&&";
        public const string ParamKeyValueSeperator = "=";
    }
}
