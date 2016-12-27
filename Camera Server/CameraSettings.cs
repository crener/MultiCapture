using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Camera_Server
{
    public static class CameraSettings
    {
        private static string saveLocation = Path.GetPathRoot(Directory.GetCurrentDirectory()) +
                                                     "scanimage" + Path.DirectorySeparatorChar + "configuration.conf";
        private static Dictionary<string, string> settings = new Dictionary<string, string>();
        static CameraSettings()
        {
            if (settings.Count > 0) return;

            if (!Load())
            {
                Defaults();
            }
        }

        private static void Defaults()
        {
            settings.Add("name", "PiCam");
            settings.Add("port", 11003.ToString());
        }

        private static bool Load()
        {
            try
            {
                if (!File.Exists(saveLocation)) return false;

                string[] fileContents = { "" };
                File.ReadAllLines(saveLocation);

                foreach (string pair in fileContents)
                {
                    string[] seperated = Regex.Split(pair, "=");
                    settings.Add(seperated[0], seperated[1]);
                }
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private static void Save()
        {
            using (FileStream file = new FileStream(saveLocation, FileMode.Truncate))
            {
                foreach (KeyValuePair<string, string> setting in settings)
                {
                    file.Write(Encoding.ASCII.GetBytes(setting.Key + "=" + setting.Value),
                        0, int.MaxValue);
                }
            }
        }

        public static void AddSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
            {
                settings.Remove(key);
                settings.Add(key, value);
            }
            else settings.Add(key, value);

            try
            {
                Save();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not save Message: " + e.Message);
            }
        }

        /// <summary>
        /// try to get a setting
        /// </summary>
        /// <param name="key">setting name</param>
        /// <returns></returns>
        public static string GetSetting(string key)
        {
            return settings[key];
        }

        /// <summary>
        /// try to get a setting if it doesn't exist return the defualt value
        /// </summary>
        /// <param name="key">setting name</param>
        /// <param name="defaultValue">if setting is not avaliable this will be returned</param>
        /// <returns></returns>
        public static string GetSetting(string key, string defaultValue)
        {
            string value;
            if (settings.TryGetValue(key, out value))
                return value;
            return defaultValue;
        }
    }
}
