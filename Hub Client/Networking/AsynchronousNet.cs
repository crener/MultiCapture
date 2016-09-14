using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using SharedDeviceItems;

namespace Hub.Networking
{
    public class AsynchronousNet : INetwork
    {
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private ManualResetEvent sentData = new ManualResetEvent(false),
            recieveData = new ManualResetEvent(false);

        public byte[] MakeRequest(ISocket socket, CameraRequest request)
        {
            //check for valid input
            if (!socket.Connected) throw new Exception("Socket needs to be connnected");

            //send the request to the camera
            string sendRequest = ((int)request) + Constants.EndOfMessage;
            SendData(socket, sendRequest);
            sentData.WaitOne();

            //wait for the request data
            StateObject state = new StateObject { WorkSocket = socket };
            try
            {
                socket.BeginReceive(state.Buffer, 0, Constants.ByteArraySize, 0, ReceiveDataComplete, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            recieveData.WaitOne();

            return state.Buffer;
        }

        private void SendData(ISocket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, SendDataComplete, client);
        }

        private void SendDataComplete(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sentData.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveDataComplete(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                ISocket client = state.WorkSocket;
                int bytesRead = client.EndReceive(ar);

                //put data into the buffer
                if (bytesRead > 0)
                {
                    client.BeginReceive(state.Buffer, state.Saved, bytesRead, 0, ReceiveDataComplete, state);
                    if (Encoding.ASCII.GetString(state.Buffer, 0, bytesRead) == Constants.FailString) Console.WriteLine("Data request failed");
                    state.Saved += bytesRead;
                    Console.WriteLine("read " + bytesRead + " bytes");
                    Console.WriteLine(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                }
                else
                {
                    Console.WriteLine("Total data recieved " + state.Saved + "bytes");
                    recieveData.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        #region MS code
        /*public void StartClient(uint staticIpAddress = 2668101289, int port = 11003)
        {
            // Connect to a remote device.
            try
            {
                //setup Ip connection
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = new IPAddress(staticIpAddress);
                IPEndPoint remoteEp = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEp, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.
                SendData(client, (int)CameraRequest.SendTestImage + Constants.EndOfMessage);
                sentData.WaitOne();

                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(ISocket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.WorkSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.Buffer, 0, Constants.ByteArraySize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                ISocket client = state.WorkSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                //cram all data into the buffer
                if (bytesRead > 0)
                {
                    client.BeginReceive(state.Buffer, state.Saved, bytesRead, 0, ReceiveCallback, state);

                    if (Encoding.ASCII.GetString(state.Buffer, 0, bytesRead) == Constants.FailString) Console.WriteLine("Data request failed");

                    state.Saved += bytesRead;
                    Console.WriteLine("read " + bytesRead + " bytes");
                }
                else
                {
                    Console.WriteLine("Total data recieved " + state.Saved + "bytes");
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }*/
        #endregion
    }
}