using System;
using Hub.DesktopInterconnect;
using Hub.Helpers;
using Hub.Helpers.Interface;
using Hub.Threaded;

namespace Hub.Util
{
    /// <summary>
    /// Correctly initializes and stores all the resources required for correct operation
    /// </summary>
    public static class Deployer
    {
        public static ICameraManager Manager { get; private set; }

        public static SaveLoad.Data SysConfig
        {
            get { return SaveLoad.Conf; }
            set
            {
                SaveLoad.Conf = value;
                SaveLoad.Save();
            }
        }

        public static void Start()
        {
            Logger.Logger logs = new Logger.Logger();
            logs.RemoveOldLogs(DateTime.Today.AddMonths(-1));

            SysConfig = SaveLoad.Load();
            DesktopThread.Instance.Start();
            Manager = new TaskManager(SysConfig);
        }
    }
}
