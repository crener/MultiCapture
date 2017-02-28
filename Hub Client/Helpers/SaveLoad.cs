using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Hub.Helpers
{
    /// <summary>
    /// Container for multipul camera configurations and other data.
    /// </summary>
    public static class SaveContainer
    {
        public static Data Conf { get; set; }

        private static string defaultSaveDirectory = Path.GetPathRoot(Directory.GetCurrentDirectory()) +
                                                     "scanimage" + Path.DirectorySeparatorChar + "configuration.conf";

        private static string customSaveDirectory = null;


        /// <summary>
        /// Load the configuration file using the default location
        /// </summary>
        /// <returns>configuration data structure</returns>
        public static Data Load()
        {
            try
            {
                return Load(string.IsNullOrEmpty(CustomSaveDirectory) ? DefaultSaveDirectory : CustomSaveDirectory);
            }
            catch (InvalidDataException)
            {
                return new Data().Default();
            }
        }

        /// <summary>
        /// Load the configuration file
        /// </summary>
        /// <param name="path">Place were the configuration file is stored</param>
        /// <returns>configuration data structure</returns>
        public static Data Load(string path)
        {
            try
            {
                if (!File.Exists(path)) throw new InvalidDataException();
            }
            catch (Exception)
            {
                throw new InvalidDataException();
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = null;
            try
            {
                stream = File.OpenRead(path);
                Conf = (Data)formatter.Deserialize(stream);
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return Conf;
        }

        /// <summary>
        /// Write the configuration to file using the default location
        /// </summary>
        public static void Save()
        {
            Save(string.IsNullOrEmpty(CustomSaveDirectory) ? DefaultSaveDirectory : CustomSaveDirectory);
        }

        /// <summary>
        /// Write the configuration to file
        /// </summary>
        /// <param name="path">Place to save the configuration file</param>
        public static void Save(string path)
        {
            if (Conf.CameraCount == 0) throw new NullReferenceException();
            if (path == null) throw new NullReferenceException();

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = null;
            try
            {
                stream = File.Create(path);
                formatter.Serialize(stream, Conf);
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }

        public static string DefaultSaveDirectory
        {
            get { return defaultSaveDirectory; }
        }

        public static string CustomSaveDirectory
        {
            get { return customSaveDirectory; }
            set { customSaveDirectory = value; }
        }

        /// <summary>
        /// Structure to hold various configurations
        /// </summary>
        [Serializable]
        public struct Data : IEquatable<Data>
        {
            public CameraConfiguration[] Cameras { get; set; }
            public int CameraCount => Cameras == null ? 0 : Cameras.Length;
            public Data Default()
            {
                List<CameraConfiguration> cameras = new List<CameraConfiguration>();

                //Pi3
                cameras.Add( new CameraConfiguration
                {
                    //Address = 25798848,
                    //Address = 763996352,
                    Address = 2668101289,

                    //Address = 3190423209,
                    CamFileIdentity = "hub",
                    Port = 11003,
                    Id = 0
                });
/*
                //Zero1
                cameras.Add( new CameraConfiguration()
                {
                    Address = 108736,
                    CamFileIdentity = "1Zero",
                    Port = 11004,
                    Id = 1
                });


                //Zero2
                cameras.Add( new CameraConfiguration()
                {
                    Address = 16885952,
                    CamFileIdentity = "2Zero",
                    Port = 11005,
                    Id = 2
                });

                //Zero3
                Cameras[3] = new CameraConfiguration()
                {
                    Address = 2668101289,
                    CamFileIdentity = "0",
                    Port = 11006,
                    Id = 3
                };*/

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
    /// class whcih stores configuration for talking to camera servers
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