using System;
using System.IO;
using SharedDeviceItems;

namespace Logger
{
    /// <summary>
    /// Responsible for making console write write to file as well as writing to file
    /// </summary>
    public class Logger : IDisposable
    {
        TextWriter standard;
        DateWriter file;
        string path;
        public string Path { get; private set; }

        public Logger()
        {
            path = Constants.DefaultHubSaveLocation() + "Log" + System.IO.Path.DirectorySeparatorChar;
            Init();
        }

        public Logger(string path)
        {
            this.path = path;
            Init();
        }

        private void Init()
        {
            standard = Console.Out;

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Path = path + DateTime.Today.ToString("dd.MM.yyyy") + ".txt";

            StreamWriter writer = new StreamWriter(Path, true, standard.Encoding) {AutoFlush = true};
            file = new DateWriter(writer);

#if DEBUG
            Console.SetOut(new DualWriter(file, Console.Out));
#else
            Console.WriteLine("This version does not log to console!");
            Console.WriteLine("Look at the logs: {0}", Path);
            Console.SetOut(file);
#endif
            Console.WriteLine("Logger init done");
        }

        public Logger(TextWriter primary, string path)
        {
            this.path = path;
            standard = Console.Out;

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#if DEBUG
            Console.SetOut(new DualWriter(file, Console.Out));
#else
            Console.WriteLine("This version of the logger does not log to console!");
            Console.SetOut(file);
#endif
        }

        public void Restore()
        {
            if (Console.IsOutputRedirected)
            {
                Console.SetOut(standard);
            }
        }

        public void RemoveOldLogs(DateTime maxAge)
        {
            string[] dir;
            try
            {
                dir = Directory.GetFiles(path);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return;
            }

            int total = 0;
            foreach (string logFile in dir)
            {
                DateTime fileDate = File.GetCreationTime(logFile);
                if (fileDate < maxAge)
                {
                    File.Delete(logFile);
                    total++;
                }
            }

            if (total > 0) Console.WriteLine("Removed {0} old log files", total);
        }

        public void Dispose()
        {
            if (file != null) file.Close();
            Restore();
        }
    }
}
