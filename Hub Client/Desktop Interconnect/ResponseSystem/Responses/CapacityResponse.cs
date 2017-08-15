using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Util;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.Capacity)]
    class CapacityResponse : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            if(!SystemReady())
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?System not ready");

            string root = Path.GetPathRoot(getPath());

            DriveInfo driveInfo;
            try
            {
                driveInfo = DriveInfo.GetDrives().First(drive => drive.Name == root);
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine("The SaveDrive could not be evaluated, resorting to default drive");

                root = Path.GetPathRoot(Environment.SystemDirectory);
                try
                {
                    driveInfo = DriveInfo.GetDrives().First(drive => drive.Name == root);
                }
                catch(InvalidOperationException in2)
                {
                    Console.WriteLine("Backup plan failed! Message: " + in2.Message);
#if DEBUG
                    Console.WriteLine(in2.StackTrace);
#endif

                    return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?Failed to gather drive data, see logs for more information!");
                }
            }

            decimal freeMB = driveInfo.TotalFreeSpace / 1000000m;

            Console.WriteLine("Remaining Capacity: {0}MB", freeMB.ToString("N"));
            return Encoding.ASCII.GetBytes(ResponseConstants.SuccessString + "?" + freeMB.ToString("F3"));
        }

        protected virtual string getPath()
        {
            return Deployer.Manager.SavePath;
        }

        protected virtual bool SystemReady()
        {
            return Deployer.Manager != null;
        }
    }
}
