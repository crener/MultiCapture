using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hub_Client.Desktop_Interconnect
{
    /// <summary>
    /// Responsible for handling commands given from a desktop connected desktop
    /// </summary>
    class DesktopConnection
    {
        private const int bufferSize = 1024;
        byte[] readBuffer = new byte[bufferSize];

        internal DesktopConnection(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            stream.BeginRead(readBuffer, 0, bufferSize, NewInstructionCallback, stream);

            PollClient(client);
        }

        private void NewInstructionCallback(IAsyncResult ar)
        {
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            int read = stream.EndRead(ar);

            if (read == 0) return;

            string instruction = Encoding.ASCII.GetString(readBuffer, 0, read);
            switch (instruction)
            {
                default:
                    Console.WriteLine("Unknown Desktop instruction received: \"{0}\"", instruction);
                    break;
            }

        }

        /// <summary>
        /// Ensure the connection is still active
        /// </summary>
        /// <param name="clientSocket">connected socket</param>
        private async void PollClient(TcpClient clientSocket)
        {
            bool done = false;

            do
            {
                await Task.Delay(5000);
                await Task.Run(() =>
                {
                    try
                    {
                        done = !(!clientSocket.Client.Poll(1, SelectMode.SelectRead) &&
                                 clientSocket.Available == 0);
                    }
                    catch (SocketException)
                    {
                        done = false;
                    }
                });
            } while (!done);

            DesktopThread.Disconnected();
        }
    }
}