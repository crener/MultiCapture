using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Util;
using Newtonsoft.Json;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.getRecentLogFile),
     ResponseType(ScannerCommands.getRecentLogDiff)]
    internal class LogResponse : BaseResponse
    {
        private long lastLogPosition = -1;

        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            if (command == ScannerCommands.getRecentLogFile)
            {
                return LogFull();
            }
            else if (command == ScannerCommands.getRecentLogDiff)
            {
                return LogDiff();
            }

            throw new UnknownResponseException();
        }

        private byte[] LogFull()
        {
            try
            {
                string path = Deployer.Log.Path;
                if (File.Exists(path))
                {
                    string line;
                    List<string> lines = new List<string>();

                    using (FileStream file =
                        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(file, Encoding.Default))
                        {
                            while ((line = sr.ReadLine()) != null)
                                lines.Add(line);
                        }
                    }

                    lastLogPosition = lines.Count;

                    string json = JsonConvert.SerializeObject(lines);
                    return Encoding.ASCII.GetBytes(json);
                }
                else
                {
                    Console.WriteLine("Log file not found! Path: " + path);
                }
            }
            catch (IOException io)
            {
                Console.WriteLine("Error Reading File! " + io.Message);
#if DEBUG
                Console.WriteLine(io);
#endif
            }

            return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?Unable to read log file");
        }

        private byte[] LogDiff()
        {
            if (lastLogPosition == -1)
            {
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?No log position, get the full log first!");
            }

            try
            {
                string path = Deployer.Log.Path;
                if (File.Exists(path))
                {
                    string line;
                    int lineCount = 0;
                    List<string> lines = new List<string>();

                    using (FileStream file =
                        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(file, Encoding.Default))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                ++lineCount;

                                if (lineCount <= lastLogPosition) continue;
                                lines.Add(line);
                            }
                        }
                    }

                    lastLogPosition += lines.Count;

                    string json = JsonConvert.SerializeObject(lines);
                    return Encoding.ASCII.GetBytes(json);
                }
                else
                {
                    Console.WriteLine("Log file not found! Path: " + path);
                }
            }
            catch (IOException io)
            {
                Console.WriteLine("Error Reading File! " + io.Message);
#if DEBUG
                Console.WriteLine(io);
#endif
            }

            return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?Unable to read log file");
        }

        public override void Reset()
        {
            lastLogPosition = -1;
        }
    }
}
