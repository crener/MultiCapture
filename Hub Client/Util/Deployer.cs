using System;
using Hub.DesktopInterconnect;
using Hub.Helpers;
using Hub.Helpers.Interface;
using Hub.Threaded;

namespace Hub.Util
{

    //responsible for correctly initializing all the resources required for correct operation
    class Deployer
    {
        public static Deployer Inst
        {
            get
            {
                if (inst == null) new Deployer();
                return inst;
            }
        }
        private static Deployer inst;

        public ICameraManager Manager { get; private set; }

        public SaveLoad.Data SysConfig
        {
            get { return SaveLoad.Conf; }
            set
            {
                SaveLoad.Conf = value;
                SaveLoad.Save();
            }
        }

        public Deployer()
        {
            inst = this;
            Logger.Logger logs = new Logger.Logger();
            logs.RemoveOldLogs(DateTime.Today.AddMonths(-1));

            SysConfig = SaveLoad.Load();
            DesktopThread.Start();
            Manager = new TaskManager(SysConfig);
        }
    }
}
