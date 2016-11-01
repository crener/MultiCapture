using System;
using Hub.Helpers;
using Hub_Client.Threaded;
using SharedDeviceItems;

namespace Hub
{
    public class HubMain
    {
        public static int Main(String[] args)
        {
            HubMain program = new HubMain();
            program.Start();
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
            ThreadManager manager = new ThreadManager(SaveContainer.Load());

            string command = "";
            while ((command = Console.ReadLine()) != "e")
            {
                if(command == "t") manager.CaptureImageSet(CameraRequest.SendTestImage);
                else manager.CaptureImageSet();
            }
            Console.WriteLine("Quitting");
        }
    }
}