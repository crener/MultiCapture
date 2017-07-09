namespace SharedDeviceItems.Networking.CameraHubConnection
{
    interface IRequester
    {
        byte[] Request(CameraRequest request);
        byte[] Request(byte[] requestData);
    }
}
