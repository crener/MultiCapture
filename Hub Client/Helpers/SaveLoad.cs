using System;
using System.CodeDom;
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

        public static string DefaultSaveDirectory = Path.AltDirectorySeparatorChar + "scanImage" +
            Path.AltDirectorySeparatorChar + "configuration.conf";


        /// <summary>
        /// Load the configuration file using the default location
        /// </summary>
        /// <returns>configuration data structure</returns>
        public static Data Load()
        {
            try
            {
                return Load(DefaultSaveDirectory);
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
            if (!File.Exists(path)) throw new InvalidDataException();

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
            Save(DefaultSaveDirectory);
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

        /// <summary>
        /// Structure to hold various configurations
        /// </summary>
        [Serializable]
        public struct Data
        {
            public CameraConfiguration[] Cameras { get; set; }
            public int CameraCount => Cameras == null ? 0 : Cameras.Length;
            public Data Default()
            {
                Cameras = new CameraConfiguration[1];

                //Pi3
                Cameras[0] = new CameraConfiguration
                {
                    Address = 1401530560,
                    //Address = 3190423209,
                    CamFileIdentity = "0",
                    Port = 11003,
                    Id = 0
                };

                //Zero1
                /*Cameras[1] = new CameraConfiguration()
                {
                    Address = 2668101289,
                    CamFileIdentity = "0",
                    Port = 110004,
                    Id = 1
                };


                //Zero2
                Cameras[2] = new CameraConfiguration()
                {
                    Address = 2668101289,
                    CamFileIdentity = "0",
                    Port = 110005,
                    Id = 2
                };

                //Zero3
                Cameras[3] = new CameraConfiguration()
                {
                    Address = 2668101289,
                    CamFileIdentity = "0",
                    Port = 110006,
                    Id = 3
                };*/

                return this;
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