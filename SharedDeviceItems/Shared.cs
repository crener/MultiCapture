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
        public static string EndOfMessage = "xtx";
        public static string MessageSeperator = "<,,,>";
        public static string FailString = "FAIL";
    }
}
