namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public interface IRequester
    {
        byte[] Request(CameraRequest request);
        byte[] Request(byte[] requestData);

        int ClearSocket();
    }
}
