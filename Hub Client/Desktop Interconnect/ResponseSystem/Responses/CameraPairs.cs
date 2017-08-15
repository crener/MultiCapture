using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hub.DesktopInterconnect;
using Newtonsoft.Json;
using SharedDeviceItems;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.CameraPairs)]
    internal class CameraPairs : BaseResponse
    {
        private readonly string pairs;

        public CameraPairs() : base()
        {
            string path = Constants.DefaultHubSaveLocation() + "pairs.json";

            if (File.Exists(path))
            {
                pairs = File.ReadAllText(path);

                //check that the camera data is valid
                try
                {
                    CameraPair[] savedpairs = JsonConvert.DeserializeObject<CameraPair[]>(pairs);

                    //perform sanity checks
                    bool valid = true;
                    List<KeyValuePair<int, int>> LRPairs = new List<KeyValuePair<int, int>>(savedpairs.Length);
                    List<KeyValuePair<int, int>> RLPairs = new List<KeyValuePair<int, int>>(savedpairs.Length);
                    foreach (CameraPair pair in savedpairs)
                    {
                        KeyValuePair<int, int> LR = new KeyValuePair<int, int>(pair.LeftCamera, pair.RightCamera);
                        KeyValuePair<int, int> RL = new KeyValuePair<int, int>(pair.RightCamera, pair.LeftCamera);

                        if (LRPairs.Contains(LR))
                        {
                            Console.WriteLine("Duplicate camera pair detected! L: " + pair.LeftCamera + ", R: " +
                                              pair.RightCamera + " is defined twice!");
                            valid = false;
                            continue;
                        }

                        LRPairs.Add(LR);
                        RLPairs.Add(RL);

                        if (LRPairs.Contains(RL) || RLPairs.Contains(LR))
                        {
                            Console.WriteLine("Flipped camera pair detected! L: " + pair.LeftCamera + ", R: " +
                                              pair.RightCamera);
                            valid = false;
                        }
                    }

                    if (!valid)
                    {
                        Console.WriteLine("Due to misconfigured camera pairs the configuration will not be used. Please fix the configuration issues.");
                        pairs = null;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Camera Pair data is invalid, delete the file to regenerate a default configuration");
                    pairs = null;
                }
            }
            else
            {
                CameraPair[] defaultPairs = {
                    new CameraPair {
                        LeftCamera = 1,
                        RightCamera = 0
                    }
                };

                pairs = JsonConvert.SerializeObject(defaultPairs);

                try
                {
                    File.WriteAllText(path, pairs, Encoding.ASCII);
                }
                catch (IOException io)
                {
                    Console.WriteLine("Could not save default camera pair configuration due to error. Message: " + io.Message);
#if DEBUG
                    Console.WriteLine(io);
#endif
                }
            }
        }

        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            return pairs == null ? Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?Camera Pair Configuration Invalid") : Encoding.ASCII.GetBytes(pairs);
        }

        private struct CameraPair
        {
            public int LeftCamera { get; set; }
            public int RightCamera { get; set; }
        }
    }
}
