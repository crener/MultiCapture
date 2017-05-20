using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub.Util;
using static System.String;

namespace Hub.DesktopInterconnect
{
    /// <summary>
    /// Scanner command response codes
    /// </summary>
    public enum ScannerCommands
    {
        Unknown = 0,

        //Global Commands
        setName = 10,
        setProjectNiceName = 11,
        getRecentLogFile = 12,
        getLoadedProjects = 13,
        getCameraConfiguration = 14,
        getCapacity = 15,
        getApiVersion = 18,

        //Camera Commands
        CaptureImageSet = 20,

        //Project Management Commands
        RemoveProject = 30,
        getAllImageSets = 31,
        getImageSet = 32,
        getProjectStats = 33
    }

    /// <summary>
    /// Responsible for handling commands given from a desktop connected desktop
    /// </summary>
    public class DesktopConnection
    {
        private const int BufferSize = 1024;
        private const char Separator = '&';
        private const char Splitter = '?';
        private const float ApiVersion = 1f;

        private const string SuccessString = "Success";
        private const string FailString = "Fail";
        private readonly byte[] successResponse = Encoding.ASCII.GetBytes(SuccessString);
        private readonly byte[] apiResponse = Encoding.ASCII.GetBytes(ApiVersion.ToString("F"));

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
            ExtractRequest(instruction, stream);
        }

        private void ExtractRequest(string instruction, NetworkStream stream)
        {
            string rawCommand = instruction.Substring(0, instruction.IndexOf(Splitter));
            ScannerCommands command = ScannerCommands.Unknown;
            if (!IsNullOrEmpty(rawCommand)) command = (ScannerCommands)Enum.Parse(typeof(ScannerCommands), rawCommand);

            if(command == ScannerCommands.Unknown)
            {
                Console.WriteLine("Unknown Desktop instruction received: \"{0}\"", instruction);
                SendResponse(stream, FailString + "?Unknown command: " + rawCommand);
                return;
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (instruction.Contains(Splitter.ToString()))
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

            ProcessRequest(command, parameters, stream);
        }


        private void ProcessRequest(ScannerCommands command, Dictionary<string, string> parameters, NetworkStream stream)
        {
            switch (command)
            {
                case ScannerCommands.setName:
                    if (!parameters.ContainsKey("name"))
                    {
                        Console.WriteLine("Name change instruction missing parameter: name");
                        SendResponse(stream, FailString + "?\"name\" parameter missing");
                        break;
                    }

                    SaveLoad.Data newConf = Deployer.SysConfig;
                    newConf.name = parameters["name"];
                    Deployer.SysConfig = newConf;

                    Console.WriteLine("Scanner Name updated to: {0}", parameters["name"]);
                    SendResponse(stream, successResponse);
                    break;
                case ScannerCommands.getApiVersion:
                    SendResponse(stream, apiResponse);
                    break;
                default:
                    Console.WriteLine("Unknown Desktop instruction received: \"{0}\"", command);
                    SendResponse(stream, FailString + "?Unknown command: " + parameters);
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

            DesktopThread.Instance.Disconnected();
        }

        private void SendResponse(NetworkStream stream, string data)
        {
            SendResponse(stream, Encoding.ASCII.GetBytes(data));
        }

        private void SendResponse(NetworkStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}