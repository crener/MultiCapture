using System;
using Hub.Helpers;
using Hub.Threaded;
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
            Logger.Logger logs = new Logger.Logger();
            logs.RemoveOldLogs(DateTime.Today.AddMonths(-1));

            string command = "";
            while ((command = Console.ReadLine()) != "e")
            {
                if(command == "t") manager.CaptureImageSet(CameraRequest.SendTestImage);
                else if(command == "s")
                {
                    Console.WriteLine(manager.SavePath);
                    Console.Write("new save path: ");
                    manager.SavePath = Console.ReadLine();
                }
                else manager.CaptureImageSet();
            }
            Console.WriteLine("Quitting");
        }
    }
}