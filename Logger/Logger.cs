using System;
using System.Text;
using System.IO;
using SharedDeviceItems;

namespace Logger
{
    /// <summary>
    /// Responsible for making console write write to file as well as writing to file
    /// </summary>
    public class Logger
    {
        TextWriter standard;
        StreamWriter file;
        string path;

        public Logger()
        {
            standard = Console.Out;
            path = Constants.DefualtHubSaveLocation() + Path.DirectorySeparatorChar + "Log" +
                Path.DirectorySeparatorChar;
            string filePath = path + DateTime.Today.ToString("d") + ".txt";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            file = new StreamWriter(filePath, true, Encoding.ASCII);
            file.AutoFlush = true;
            Console.SetOut(new DualWriter(file, Console.Out));
        }

        public Logger(string path)
        {
            this.path = path;
            standard = Console.Out;

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            file = new StreamWriter(path + DateTime.Today.ToString("d") + ".txt", true, standard.Encoding);
            file.AutoFlush = true;
            Console.SetOut(new DualWriter(file, Console.Out));
        }

        public Logger(TextWriter primary, string path)
        {
            this.path = path;
            standard = Console.Out;

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Console.SetOut(new DualWriter(primary, Console.Out));
        }

        ~Logger()
        {
            if (file != null) file.Close();
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
    }
}
