﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SharedDeviceItems;

namespace Hub.Util
{
    /// <summary>
    /// Responsible for maintaining the project file which keeps track of images and if they have been sent to a 
    /// computer for further processing.
    /// </summary>
    public class ProjectMapper
    {
        /*
         * Project File Structure
         * 
         * root
         *  ->project ID
         *  ->host project name (the name that the computer has given this project)
         *  ->version (version of the logger that last saved the project)
         *  ->done (bool, true if the project thinks that all the images have been sent)
         *  ->image sets (array of the image set data)
         *  <Begin loop for every image set>
         *      -->ImageSetID (int, represents the ID of the image set)
         *      -->file (Name of the file which contains the image set)
         *      -->all sent (bool, true if all the containing images have been sent)
         *      -->images (Array of the images that contain the image data)
         *      <Begin loop for every image in the image set>
         *          --->name (string, the name of the file that is the image)
         *          --->sent (bool, if the project thinks that the image has been sent before)
         *          --->last send date (long, unix time for the last date were the image was sent)
         *      <End loop for every image in the image set>
         *  <end loop for every image set>
         *  
         * The project file should be saved in the directory containing all the image sets for the project,
         * all image set (and by extention images) paths are relitive to this project file. Moving it without
         * the image sets might not be a good idea as all paths will be broken and the project might not be 
         * able to recover.
         * 
         * In order to get the path for an image it should be the following
         * <path to the project file>/<image set, file>/<image name>
         * C://scanpath/project234312/imageSet2/Hub2.jpg
         */
        
        private const int Version = 3;
        public const string FileName = "project.json";
        
        public Data saveData { get { return data; } }
        protected Data data = new Data(); 
        private Dictionary<int, int> setLookUp = new Dictionary<int, int>();

        public int ImageSetCount => data.sets.Count;
        public int CameraCount => data.cameras.Count;
        public int ProjectId => data.ProjectId;
        public string Name => data.ProjectName;
        string projectRoot { get; set; }
        private string projectFileLocation { get { return projectRoot + Path.DirectorySeparatorChar + FileName; } }

        /// <summary>
        /// Standard class which keeps track of project related resources such as images, project name, cameras, etc.
        /// if the project location is invalid a new location will be generated based on the projectId
        /// </summary>
        /// <param name="projectLocation">location of the project file</param>
        /// <param name="projectId">unique id of the project</param>
        public ProjectMapper(string projectLocation, int projectId)
        {
            if (projectLocation.EndsWith(FileName)) projectRoot = projectLocation.Substring(0, projectLocation.Length - FileName.Length - 1);
            else projectRoot = Constants.DefaultHubSaveLocation() + projectId;

            if (File.Exists(projectFileLocation)) Load(projectFileLocation);
            else data.ProjectId = projectId;
        }


        /// <summary>
        /// Add a new image to an existing image set
        /// </summary>
        /// <param name="setId">Image set that the image gets added too</param>
        /// <param name="name">Name of the image</param>
        /// <param name="camId">the Id of the camera that took the image</param>
        public void AddImage(int setId, string name, int camId)
        {
            if (ImageExists(setId, name)) throw new Exception("Image already exists");

            data.sets[setLookUp[setId]].Images.Add(new Image() { File = name, CameraId = camId });
        }

        /// <summary>
        /// adds a new image set
        /// </summary>
        /// <param name="setId">Image set that the image gets added too</param>
        /// <param name="path">the name of the folder containing all the images</param>
        public void AddImageSet(int setId, string path)
        {
            if (ImageSetExists(setId)) throw new KeyNotFoundException("Image Set already exists");
            if (path == null) throw new NullReferenceException("Path cannot be null");

            setLookUp.Add(setId, data.sets.Count);
            data.sets.Add(new ImageSet { ImageSetId = setId, Path = path });
        }

        /// <summary>
        /// Add a new camera to the project file
        /// </summary>
        /// <param name="id">id of the camera</param>
        /// <param name="name">nickname of the camera</param>
        public void AddCamera(int id, string name)
        {
            Camera newCam = new Camera
            {
                CameraId = id,
                CameraName = name
            };

            if (data.cameras.Contains(newCam)) throw new Exception("Camera already exists");

            data.cameras.Add(newCam);
        }

        public void Sent(int setId, string imageName)
        {
            Image look;
            if (!ImageExists(setId, imageName, out look)) throw new Exception("Image doesn't exist");

            look.Sent = true;
            look.SendDate = DateTime.Now.ToFileTimeUtc();
        }

        private void Sent(int setId, string imageName, long sendDate)
        {
            Image look;
            if (!ImageExists(setId, imageName, out look)) throw new Exception("Image doesn't exist");

            look.Sent = true;
            look.SendDate = sendDate;
        }

        public string AbsoluteImagePath(int setId, string imageName)
        {
            Image look;
            if (!ImageExists(setId, imageName, out look)) throw new Exception("Image doesn't exist");

            return projectRoot + Path.DirectorySeparatorChar + data.sets[setLookUp[setId]].Path + Path.DirectorySeparatorChar + imageName;
        }

        public static string AbsoluteImagePath(Data details, int set, string imageName)
        {
            ImageSet imSet = details.sets.First(i => i.ImageSetId == set);

            return Constants.DefaultHubSaveLocation() + details.ProjectId + Path.DirectorySeparatorChar + imSet.Path  + Path.DirectorySeparatorChar + imageName;
        }

        public bool hasSent(int setId, string imageName)
        {
            Image look;
            if (!ImageExists(setId, imageName, out look)) throw new Exception("Image doesn't exist");
            return look.Sent;
        }

        public bool hasSent(int setId)
        {
            if (!ImageSetExists(setId)) throw new Exception("Image Set doesn't exist");
            return data.sets[setId].AllSent;
        }

        public long SendTime(int setId, string imageName)
        {
            Image look;
            if (!ImageExists(setId, imageName, out look)) throw new Exception("Image doesn't exist");
            return look.SendDate;
        }

        /// <summary>
        /// Returns true if all images in the image set have been sent
        /// </summary>
        /// <param name="setId">image set id</param>
        /// <returns>true if all images in the set have been sent</returns>
        public bool ImageSetIsDone(int setId)
        {
            if (!ImageSetExists(setId)) throw new KeyNotFoundException("Image Set not found");
            if (data.sets[setLookUp[setId]].Images.Count <= 0) throw new NullReferenceException("No Images in set");

            foreach (Image img in data.sets[setLookUp[setId]].Images) if (img.Sent == false) return false;
            return true;
        }

        private bool ImageSetExists(int setId)
        {
            if (setLookUp.ContainsKey(setId)) return true;
            return false;
        }

        private bool ImageExists(int setId, string imageName)
        {
            if (!ImageSetExists(setId)) throw new KeyNotFoundException("Image Set not found");

            foreach (Image img in data.sets[setLookUp[setId]].Images)
            {
                if (img.File == imageName) return true;
            }

            return false;
        }

        private bool ImageExists(int setId, string imageName, out Image result)
        {
            if (!ImageSetExists(setId)) throw new KeyNotFoundException("Image Set not found");

            foreach (Image img in data.sets[setLookUp[setId]].Images)
            {
                if (img.File == imageName)
                {
                    result = img;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public int ImageCount(int setId)
        {
            if (!ImageSetExists(setId)) throw new KeyNotFoundException("Image Set not found");

            return data.sets[setLookUp[setId]].Images.Count;
        }

        public void Save()
        {
            if(!Directory.Exists(projectRoot)) Directory.CreateDirectory(projectRoot);

            using (StreamWriter stream = new StreamWriter(projectFileLocation))
            {
                stream.WriteLine(JsonConvert.SerializeObject(data));
            }
        }

        private void Load(string path)
        {
            data = ExtractSaveData(path);

            //reconstruct lookup data
            for(int i = 0; i < data.sets.Count; i++)
            {
                setLookUp.Add(data.sets[i].ImageSetId,i);
            }
        }

        public static Data ExtractSaveData(string path)
        {
            using (StreamReader stream = new StreamReader(path))
            {
                string saved = stream.ReadToEnd();
                return JsonConvert.DeserializeObject<Data>(saved);
            }
        }

        #region data containers

        public class Data
        {
            [JsonProperty(PropertyName = "MapperVersion")]
            public int version = Version;

            [JsonProperty(PropertyName = "projectID")]
            public int ProjectId { get; set; }
            [JsonProperty(PropertyName = "projectName")]
            public string ProjectName { get; set; }

            [JsonProperty(PropertyName = "imageSets")]
            public List<ImageSet> sets = new List<ImageSet>();
            [JsonProperty(PropertyName = "cameras")]
            public List<Camera> cameras = new List<Camera>();
        }

        public class ImageSet
        {
            [JsonProperty(PropertyName = "id")]
            public int ImageSetId { get; set; }
            [JsonProperty(PropertyName = "path")]
            public string Path { get; set; } //relative

            public bool AllSent
            {
                get
                {
                    foreach (Image img in Images)
                        if (!img.Sent) return false;
                    return true;
                }
            }

            [JsonProperty(PropertyName = "images")]
            public List<Image> Images = new List<Image>();
        }

        public class Image
        {
            [JsonProperty(PropertyName = "path")]
            public string File { get; set; }
            [JsonProperty(PropertyName = "id")]
            public int CameraId { get; set; }
            [JsonProperty(PropertyName = "sent")]
            public bool Sent { get; set; }
            [JsonProperty(PropertyName = "sentDate")]
            public long SendDate { get; set; }
        }

        public struct Camera
        {
            [JsonProperty(PropertyName = "name")]
            public string CameraName { get; set; }
            [JsonProperty(PropertyName = "id")]
            public int CameraId { get; set; }
        }
        #endregion
    }
}
