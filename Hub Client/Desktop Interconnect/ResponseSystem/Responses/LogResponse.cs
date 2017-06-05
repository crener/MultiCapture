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

                        string[] array = lines.ToArray();
                        lastLogPosition = array.Length;

                        string json = JsonConvert.SerializeObject(array);
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
            else if (command == ScannerCommands.getRecentLogDiff)
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

                    string[] array = lines.ToArray();
                    string[] needed = new string[array.Length - lastLogPosition];

                    array.CopyTo(needed, lastLogPosition);
                    lastLogPosition = array.Length;

                    string json = JsonConvert.SerializeObject(lines);
                    return Encoding.ASCII.GetBytes(json);
                }
                else
                {
                    Console.WriteLine("Log file not found! Path: " + path);
                }
            }

            throw new UnknownResponseException();
        }

        public override void Reset()
        {
            lastLogPosition = -1;
        }
    }
}
