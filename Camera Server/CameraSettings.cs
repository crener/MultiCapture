using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Camera_Server
{
    public static class CameraSettings
    {
        private static string saveLocation = Path.GetPathRoot(Directory.GetCurrentDirectory()) +
                                                     "scanimage" + Path.DirectorySeparatorChar + "camera.conf";
        private static Dictionary<string, string> settings = new Dictionary<string, string>();

        public static void Init()
        {
            if (settings.Count > 0) return;

            if (!Load())
            {
                Defaults();
            }
        }

        public static void Reload()
        {
            settings.Clear();
            Init();
        }

        private static void Defaults()
        {
            settings.Add("name", "PiCam");
            settings.Add("port", "11003");
            Save();
        }

        private static bool Load()
        {
            try
            {
                if (!File.Exists(saveLocation)) return false;

                string[] fileContents = File.ReadAllLines(saveLocation);

                foreach (string pair in fileContents)
                {
                    if(pair.Length <= 0) continue;

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
                throw e;
            }

            return true;
        }

        private static void Save()
        {
            using (StreamWriter file = new StreamWriter(saveLocation))
            {
                foreach (KeyValuePair<string, string> setting in settings)
                {
                    string data = setting.Key + "=" + setting.Value + "\n";
                    file.Write(data);
                }
            }
        }

        public static bool AddSetting(string key, string value)
        {
            if(key.Contains("=") || value.Contains("=")) return false;

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
                Console.WriteLine("Could not save, Message: " + e.Message);
            }

            return true;
        }

        /// <summary>
        /// try to get a setting
        /// </summary>
        /// <param name="key">setting name</param>
        /// <returns></returns>
        public static string GetSetting(string key)
        {
            return settings.ContainsKey(key) ? settings[key] : null;
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
