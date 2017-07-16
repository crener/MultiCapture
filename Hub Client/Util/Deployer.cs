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
        public static Logger.Logger Log { get; private set; }
        public static ProjectManager ProjectManager { get; private set; }
        public static ProjectMapper CurrentProject { get; set; }

        private static bool started = false;
        internal static bool Mock = false;

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
            if (started) return;

            started = true;
            if (!Mock)
            {
                Log = new Logger.Logger();
                Log.RemoveOldLogs(DateTime.Today.AddMonths(-1));
            }

            SysConfig = SaveLoad.Load();
            if (!Mock) Manager = new TaskManager(SysConfig);
            DesktopThread.Instance.Start();

            ProjectManager = new ProjectManager();
        }
    }
}
