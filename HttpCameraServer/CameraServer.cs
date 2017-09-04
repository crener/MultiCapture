using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using CameraServer;

namespace HttpCameraServer
{
    class CameraServer
    {
        static void Main(string[] args)
        {
            Logger.Logger log = new Logger.Logger();
            log.RemoveOldLogs(DateTime.Today.AddMonths(-1));

            if(!IsAdministrator())
            {
                Console.WriteLine("This needs to be run as an Administrator");
                return;
            }

            Console.WriteLine("Starting");
            try
            {
                CameraSettings.Init();
                HttpResponder listen = new HttpResponder();
                listen.StartListening();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("CRITICAL EXCEPTION SHUTTING DOWN!!!");
            }
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
