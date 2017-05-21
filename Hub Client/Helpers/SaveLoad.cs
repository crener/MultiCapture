using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using static SharedDeviceItems.Constants;

namespace Hub.Helpers
{
    /// <summary>
    /// Container for multiple camera configurations and other data.
    /// </summary>
    public static class SaveLoad
    {
        public static Data Conf { get; set; }

        private static string defaultSaveFile = "configuration.json";

        private static string customSaveDirectory = null;


        /// <summary>
        /// Load the configuration file using the default location
        /// </summary>
        /// <returns>configuration data structure</returns>
        public static Data Load()
        {
            try
            {
                return Load(string.IsNullOrEmpty(customSaveDirectory) ? DefaultSavePath : CustomSaveDirectory);
            }
            catch (FileNotFoundException)
            {
                Conf = new Data().Default();
                Save();
                return Conf;
            }
        }

        /// <summary>
        /// Load the configuration file
        /// </summary>
        /// <param name="path">Place were the configuration file is stored</param>
        /// <returns>configuration data structure</returns>
        public static Data Load(string path)
        {

            if (!File.Exists(path)) throw new FileNotFoundException();

            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    string data = file.ReadToEnd();
                    Conf = JsonConvert.DeserializeObject<Data>(data);
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;
            }

            return Conf;
        }

        /// <summary>
        /// Write the configuration to file using the default location
        /// </summary>
        public static void Save()
        {
            Save(string.IsNullOrEmpty(customSaveDirectory) ? DefaultSavePath : CustomSaveDirectory);
        }

        /// <summary>
        /// Write the configuration to file
        /// </summary>
        /// <param name="path">Place to save the configuration file</param>
        public static void Save(string path)
        {
            if (Conf.CameraCount == 0) throw new NullReferenceException();
            if (path == null) throw new NullReferenceException();

            try
            {
                using (StreamWriter file = new StreamWriter(path))
                {
                    string data = JsonConvert.SerializeObject(Conf);
                    file.Write(data);
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;
            }
        }

        public static string DefaultSavePath
        {
            get { return DefaultHubSaveLocation() + defaultSaveFile; }
        }

        public static string CustomSaveDirectory
        {
            get { return DefaultHubSaveLocation() + customSaveDirectory; }
            set { customSaveDirectory = value; }
        }

        /// <summary>
        /// Structure to hold various configurations
        /// </summary>
        [Serializable]
        public struct Data : IEquatable<Data>
        {
            public string name { get; set; }
            public CameraConfiguration[] Cameras { get; set; }

            [JsonIgnore]
            public int CameraCount => Cameras == null ? 0 : Cameras.Length;
            public Data Default()
            {
                name = "3D Scanner";

                List<CameraConfiguration> cameras = new List<CameraConfiguration>();
                cameras.Add(new CameraConfiguration
                {
                    Address = 2668101289,
                    CamFileIdentity = "hub",
                    Port = 11003,
                    Id = 0
                });

                Cameras = cameras.ToArray();
                return this;
            }

            public bool Equals(Data other)
            {
                if (CameraCount != other.CameraCount) return false;

                for (int i = 0; i < Cameras.Length; i++)
                {
                    if (Cameras[i].Address != other.Cameras[i].Address) return false;
                    if (Cameras[i].Id != other.Cameras[i].Id) return false;
                    if (Cameras[i].Port != other.Cameras[i].Port) return false;
                    if (Cameras[i].CamFileIdentity != other.Cameras[i].CamFileIdentity) return false;
                }

                return true;
            }
        }
    }

    /// <summary>
    /// stores configuration for talking to camera servers
    /// </summary>
    [Serializable]
    public class CameraConfiguration
    {
        public long Address { get; set; }
        public int Port { get; set; }
        public int Id { get; set; }
        public string CamFileIdentity { get; set; }
    }
}