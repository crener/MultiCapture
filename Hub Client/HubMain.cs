using System;
using Hub.Helpers.Interface;
using Hub.Util;
using SharedDeviceItems;

namespace Hub
{
    public class HubMain
    {
        public static int Main(String[] args)
        {
            new HubMain();
            return 0;
        }

        public HubMain()
        {
            Start();
        }

        /// <summary>
        /// Main runtime loop used to collect and send data off too the computer
        /// </summary>
        private void Start()
        {
            ICameraManager manager = Deployer.Manager;

            string command = "";
            while ((command = Console.ReadLine()) != "e")
            {
                if (command == "t" || command == "test") manager.CaptureImageSet(CameraRequest.SendTestImage);
                else if (command == "s")
                {
                    Console.WriteLine(manager.SavePath);
                    Console.Write("new save path: ");
                    manager.SavePath = Console.ReadLine();
                }
                else if (command == "clear") manager.ClearSockets();
                else manager.CaptureImageSet();
            }
            ProjectMapper.Instance.Save();
            Console.WriteLine("Quitting");
        }
    }
}