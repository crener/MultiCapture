using System;
using System.Collections.Generic;
using SharedDeviceItems;
using SharedDeviceItems.Helpers;
using SharedDeviceItems.Interface;

namespace CameraServer
{
    public class RequestProcess
    {
        protected ICamera camera;
        private static Dictionary<string, CameraRequest> requestLookup = new Dictionary<string, CameraRequest>();

        public RequestProcess(ICamera newCamera)
        {
            camera = newCamera;
            camera.SetCameraName(CameraSettings.GetSetting("name"));
            camera.SetResolution(3280, 2464);

            if (requestLookup.Count <= 0)
            {
                Console.WriteLine("request initialisation");
                CameraRequest[] enums = (CameraRequest[])Enum.GetValues(typeof(CameraRequest));
                foreach (CameraRequest value in enums)
                {
                    Console.WriteLine("Key = " + (int)value + ", value = " + value);
                    requestLookup.Add("" + (int)value, value);
                }
            }
        }

        public virtual byte[] ProcessRequest(byte[] message)
        {
            CommandReader requestMessage = new CommandReader(message);
            return ProcessRequest(requestMessage);
        }

        public byte[] ProcessRequest(string message)
        {
            CommandReader requestMessage = new CommandReader(message);
            return ProcessRequest(requestMessage);
        }

        private byte[] ProcessRequest(CommandReader requestMessage)
        {
            switch(requestMessage.Request)
            {
                case CameraRequest.Alive:
                    return Constants.SuccessStringBytes;
                //case CameraRequest.SendCustomResImage:
                case CameraRequest.SendFullResImage:
                    return ProcessCaptureRequest(requestMessage);
                case CameraRequest.SendTestImage:
                    return ByteHelpers.FileToBytes(Constants.DefaultHubSaveLocation() + "test.jpg");
                case CameraRequest.SetProporties:
                    return ProcessSetProporties(requestMessage);
                default:
                    return Constants.FailStringBytes;
            }
        }

        private byte[] ProcessSetProporties(CommandReader command)
        {
            if(command.Parameters.ContainsKey(Constants.CameraSettingName))
            {
                CameraSettings.AddSetting(Constants.CameraSettingName, command.Parameters[Constants.CameraSettingName]);
                camera.SetCameraName(command.Parameters[Constants.CameraSettingName]);
            }

            return Constants.SuccessStringBytes;
        }

        private byte[] ProcessCaptureRequest(CommandReader command)
        {
            if(!command.Parameters.ContainsKey(Constants.CameraCaptureImageName)) return Constants.FailStringBytes;

            //todo extract image size as a parameter rather than setting statically in the constructor and set as capture parameter
            string imageName = command.Parameters[Constants.CameraCaptureImageName];
            Console.WriteLine("ImageName: " + imageName);

            if(command.Request == CameraRequest.SendFullResImage)
            {
                return camera.CaptureImageByte(imageName);
            }

            return Constants.FailStringBytes;
        }
    }
}