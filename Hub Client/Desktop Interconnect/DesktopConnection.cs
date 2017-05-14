using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub_Client.Util;
using static System.String;

namespace Hub_Client.Desktop_Interconnect
{
    /// <summary>
    /// Responsible for handling commands given from a desktop connected desktop
    /// </summary>
    class DesktopConnection
    {
        private const int BufferSize = 1024;
        private const char Separator = '&';
        private const char Splitter = '?';
        byte[] readBuffer = new byte[BufferSize];

        internal DesktopConnection(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            stream.BeginRead(readBuffer, 0, BufferSize, NewInstructionCallback, stream);

            PollClient(client);
        }

        private void NewInstructionCallback(IAsyncResult ar)
        {
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            int read;

            try
            {
                read = stream.EndRead(ar);
                if (read == 0) return;
            }
            catch (IOException io)
            {
                Console.WriteLine("Error handeling request: {0}", io.Message);
                return;
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error handeling request: {0}", e.Message);
                return;

            }

            string instruction = Encoding.ASCII.GetString(readBuffer, 0, read);
            ScannerCommands command = ScannerCommands.Unknown;
            {
                string rawCommand = instruction.Substring(0, instruction.IndexOf(Splitter));
                if (!IsNullOrEmpty(rawCommand))
                {
                    command = (ScannerCommands)Enum.Parse(typeof(ScannerCommands), rawCommand);
                }
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            {
                string rawParameters = instruction.Substring(instruction.IndexOf(Splitter) + 1);
                string[] pairs = rawParameters.Split(Separator);
                foreach (string pair in pairs)
                {
                    if (IsNullOrEmpty(pair) || !pair.Contains("=")) continue;
                    string key, value;
                    key = pair.Substring(0, pair.IndexOf('='));
                    value = pair.Substring(pair.IndexOf('=') + 1);

                    parameters.Add(key, value);
                }
            }

            switch (command)
            {
                case ScannerCommands.setName:
                    if (!parameters.ContainsKey("name"))
                    {
                        Console.WriteLine("Name change instruction missing parameter: name");
                        sendResponse(stream, Encoding.ASCII.GetBytes("Fail?name missing"));
                        break;
                    }

                    SaveLoad.Data newConf = Deployer.Inst.SysConfig;
                    newConf.name = parameters["name"];
                    Deployer.Inst.SysConfig = newConf;

                    Console.WriteLine("Scanner Name updated to: {0}", parameters["name"]);
                    sendResponse(stream, Encoding.ASCII.GetBytes("Success"));
                    break;
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

        private void sendResponse(NetworkStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }

    enum ScannerCommands
    {
        Unknown = 0,

        //Global Commands
        setName = 10,
        setProjectNiceName = 11,
        getRecentLogFile = 12,
        getLoadedProjects = 13,
        getCameraConfiguration = 14,
        getCapacity = 15,

        //Camera Commands
        CaptureImageSet = 20,

        //Project Management Commands
        RemoveProject = 30,
        getAllImageSets = 31,
        getImageSet = 32,
        getProjectStats = 33,

    };
}