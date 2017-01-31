using System;
using System.Text;
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
        StreamWriter file;
        string path;

        public Logger()
        {
            path = Constants.DefualtHubSaveLocation() + "Log" + Path.DirectorySeparatorChar;
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
            string filePath = path + DateTime.Today.ToString("dd.MM.yyyy") + ".txt";

            file = new StreamWriter(filePath, true, standard.Encoding);
            file.AutoFlush = true;
#if DEBUG
            Console.SetOut(new DualWriter(file, Console.Out));
#else
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

            foreach (string file in dir)
            {
                DateTime fileDate = File.GetCreationTime(file);
                if (fileDate < maxAge)
                {
                    File.Delete(file);
                }
            }
        }

        public void Dispose()
        {
            if (file != null) file.Close();
        }
    }
}
