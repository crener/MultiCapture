using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hub.ResponseSystem;
using static System.String;
using static Hub.DesktopInterconnect.ResponseConstants;

namespace Hub.DesktopInterconnect
{
    /// <summary>
    /// Responsible for handling commands given from a desktop connected desktop
    /// </summary>
    public class DesktopConnection
    {
        private const int BufferSize = 1024;
        private const int ConnectionCheckMs = 15000;
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
            ExtractRequest(instruction, stream);

            stream.BeginRead(readBuffer, 0, BufferSize, NewInstructionCallback, stream);
        }

        private void ExtractRequest(string instruction, NetworkStream stream)
        {
            string rawCommand = instruction.IndexOf(Splitter) >= 0 ? instruction.Substring(0, instruction.IndexOf(Splitter)) : instruction;
            ScannerCommands command = ScannerCommands.Unknown;
            if (!IsNullOrEmpty(rawCommand)) command = (ScannerCommands)Enum.Parse(typeof(ScannerCommands), rawCommand);

            if (command == ScannerCommands.Unknown)
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
            Console.WriteLine("External Instruction received: \"{0}\", code: {1}", command, (int)command);
            if (command == ScannerCommands.Unknown)
                Console.WriteLine("\tParams: " + parameters);

            if (DesktopThread.Responders.ContainsKey(command))
            {
                try
                {
                    byte[] data = DesktopThread.Responders[command].GenerateResponse(command, parameters);
                    byte[] pre = Encoding.ASCII.GetBytes("<" + (int)command + ":" + data.Length + ">");

                    byte[] message = new byte[pre.Length + data.Length];
                    pre.CopyTo(message, 0);
                    data.CopyTo(message, pre.Length);

                    SendResponse(stream, message);
                }
                catch (UnknownResponseException ex)
                {
                    Console.WriteLine("Response didn't know what to do! Response: {0}",
                        DesktopThread.Responders[command].GetType());
                    SendResponse(stream, FailString + "?Response didn't know what to do. " + parameters.ToString());
#if DEBUG
                    Console.WriteLine(ex);
#endif
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Response Threw Exception! Response: {0}, Exception: {1}, Message: {2}",
                        DesktopThread.Responders[command].GetType(), ex.GetType(), ex.Message);
#if DEBUG
                    Console.WriteLine(ex);
#endif

                    SendResponse(stream, FailString + "?Response didn't know what to do!\nType: " + ex.GetType() + "\nMessage: " + ex.Message);
                }
                return;
            }

            SendResponse(stream, FailString + "?Unknown command! " + parameters.ToString());
        }


        /// <summary>
        /// Ensure the connection is still active
        /// </summary>
        /// <param name="clientSocket">connected socket</param>
        private async void PollClient(TcpClient clientSocket)
        {
            bool disconnected = false;

            do
            {
                await Task.Delay(ConnectionCheckMs);
                await Task.Run(() =>
                {
                    try
                    {
                        disconnected = !(!clientSocket.Client.Poll(1, SelectMode.SelectRead) &&
                                 clientSocket.Available == 0);
                    }
                    catch (SocketException)
                    {
                        disconnected = false;
                    }
                });
            } while (!disconnected);

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