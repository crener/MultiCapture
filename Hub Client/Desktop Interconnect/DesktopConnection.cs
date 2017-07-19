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
    /// Responsible for handeling commands given from an external device via an already established socket connection.
    /// </summary>
    public class DesktopConnection
    {
        private const int BufferSize = 1024;
        private const int ConnectionCheckMs = 15000;
        public const char Separator = '&';
        public const char ParamSeperator = '=';

        byte[] readBuffer = new byte[BufferSize];

        protected DesktopConnection()
        {

        }

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

        /*
         * The format for a request is Command&key=value&key=value
         */
        protected virtual void ExtractRequest(string instruction, NetworkStream stream)
        {
            //extract the scanner command
            ScannerCommands command = ScannerCommands.Unknown;
            string rawCommand = instruction.IndexOf(Separator) >= 0
                ? instruction.Substring(0, instruction.IndexOf(Separator))
                : instruction;

            {
                bool success = true;

                int commandNo;
                success = int.TryParse(rawCommand, out commandNo);

                if (success &&
                   !IsNullOrEmpty(rawCommand) &&
                   Enum.IsDefined(typeof(ScannerCommands), commandNo))
                {
                    command = (ScannerCommands)Enum.Parse(typeof(ScannerCommands), rawCommand);
                }
            }

            if (command == ScannerCommands.Unknown)
            {
                Console.WriteLine("Unknown Desktop instruction received: \"{0}\"", instruction);
                SendResponse(stream, FailString + "?Unknown command: " + rawCommand);
                return;
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (instruction.Contains(Separator.ToString()))
            {
                string rawParameters = instruction.Substring(instruction.IndexOf(Separator) + 1);
                string[] pairs = rawParameters.Split(Separator);
                foreach (string pair in pairs)
                {
                    if (IsNullOrEmpty(pair) || !pair.Contains(ParamSeperator.ToString())) continue;
                    string key, value;
                    key = pair.Substring(0, pair.IndexOf(ParamSeperator));
                    value = pair.Substring(pair.IndexOf(ParamSeperator) + 1);

                    parameters.Add(key, value);
                }
            }

            ProcessRequest(command, parameters, stream);
        }


        protected virtual void ProcessRequest(ScannerCommands command, Dictionary<string, string> parameters, NetworkStream stream)
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
                    SendResponse(stream, FailString + "?Response " + DesktopThread.Responders.ContainsKey(command).GetType() + " didn't know what to do.");

                    Console.WriteLine(ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Response Threw Exception! Response: {0}, Exception: {1}, Message: {2}",
                        DesktopThread.Responders[command].GetType(), ex.GetType(), ex.Message);
                    Console.WriteLine(ex);

                    SendResponse(stream, FailString + "?Response didn't know what to do!\nType: " + ex.GetType() + "\nMessage: " + ex.Message);
                }
                return;
            }

            SendResponse(stream, FailString + "?No Response avaliable!");
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

        protected virtual void SendResponse(NetworkStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}