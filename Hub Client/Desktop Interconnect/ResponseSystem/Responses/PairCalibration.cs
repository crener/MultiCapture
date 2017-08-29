using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Helpers;

[assembly: InternalsVisibleTo("Hub ClientTests")]
namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.getCameraPairConfiguration), ResponseType(ScannerCommands.setCameraPairConfiguration)]
    internal class PairCalibration : BaseResponse
    {
        public PairCalibration() : base()
        {
            //ensure that the default path always exists
            if (!Directory.Exists(ResponseConstants.PairConfigPath))
                Directory.CreateDirectory(ResponseConstants.PairConfigPath);
        }

        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            byte[] response;
            int pairId;

            if (!ExtractParameter(command, parameters, "id", out pairId, out response))
                return response;

            if (command == ScannerCommands.getCameraPairConfiguration)
                return getPairConfiguration(pairId);
            else if (command == ScannerCommands.setCameraPairConfiguration)
            {
                string fileText;
                if (!ExtractParameter(command, parameters, "config", out fileText, out response))
                    return response;

                return NewPairConfiguration(pairId, fileText);
            }

            throw new UnknownResponseException();
        }

        private byte[] getPairConfiguration(int pairId)
        {
            List<int> knownIds = SaveLoad.Conf.cameraPairs;
            if (!knownIds.Contains(pairId))
            {
                Console.WriteLine(ScannerCommands.getCameraPairConfiguration + " has requested an invalid pair (" + pairId + ")");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?camera pair " + pairId + " is unknown");
            }

            string path = ResponseConstants.PairConfigPath + pairId + ResponseConstants.PairFileType;
            if (!File.Exists(path))
            {
                Console.WriteLine(ScannerCommands.getCameraPairConfiguration + " requested camera pair config couldn't be found! Searched: " + path);
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?camera pair configuration file was not found but should exist");
            }

            string config = File.ReadAllText(path);

            return Encoding.ASCII.GetBytes(ResponseConstants.SuccessString + "?" + config);
        }

        private byte[] NewPairConfiguration(int pairId, string fileData)
        {
            string path = ResponseConstants.PairConfigPath + pairId + ResponseConstants.PairFileType;

            try
            {
                using(StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.Write(fileData);
                }
            }
            catch(IOException io)
            {
                Console.WriteLine(ScannerCommands.setCameraPairConfiguration + "could not save due to exception. Message: " + io.Message);
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString +
                                               "? exception thrown while saving. message: " + io.Message);
            }

            SaveLoad.Conf.cameraPairs.Add(pairId);
            SaveLoad.Save();
            return ResponseConstants.SuccessResponse;
        }
    }
}
