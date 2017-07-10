using Hub.Networking;

namespace SharedDeviceItems.Networking.CameraHubConnection
{
    public interface IResponder
    {
        /// <summary>
        /// Blocking call to make a TCP connection to an incomming request
        /// </summary>
        void Connect(ISocket listeningSocket);

        /// <summary>
        /// Blocking method which returns byte data for a request once it becomes avaliable
        /// </summary>
        /// <returns>data sent from an ISender</returns>
        byte[] RecieveData();

        /// <summary>
        /// Sends data back though the socket, There must be a request first otherwise an exception will be thrown
        /// </summary>
        /// <param name="data">the data that is sent</param>
        void SendResponse(byte[] data);

        /// <summary>
        /// Clears any avaliable data from the socket and returns the amount that was cleared
        /// </summary>
        /// <returns>amount of bytes that were cleared</returns>
        int ClearSocket();

        /// <summary>
        /// Checks if the Reciever is connected to something
        /// </summary>
        /// <returns></returns>
        bool Connected();
    }
}
