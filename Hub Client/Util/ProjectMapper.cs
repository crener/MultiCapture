using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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
        string fileLocation { get; set; }

/**
        #region XML naming
        private const string ProdId = "projectID";
        private const string ProdName = "projectName";
        private const string MapperVersion = "MapperVersion";
        private const string Done = "done";

        private const string CameraGroup = "cameras";
        private const string CameraSetLabel = "camera";
        private const string CameraId = "id";
        private const string CameraName = "name";

        private const string ImageSetGroup = "imageSets";
        private const string ImageSetLabel = "set";
        private const string ImageSetGroupFile = "file";
        private const string ImageSetGroupDone = "done";
        private const string ImageSetId = "id";

        private const string ImageGroupHeader = "images";
        private const string ImageHeader = "image";
        private const string ImagePath = "path";
        private const string ImageCamera = "cam";
        private const string ImageSent = "sent";
        private const string ImageSentDate = "sentDate";
        #endregion
        **/

        public ProjectMapper(string project, int projectId)
        {
            if (project.EndsWith(FileName)) fileLocation = project;
            else fileLocation = project + Path.DirectorySeparatorChar + FileName;

            if (File.Exists(fileLocation)) Load(fileLocation);
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

            data.sets[setLookUp[setId]].images.Add(new Image() { File = name, CameraId = camId });
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

        public string ImageAbsolutePath(int setId, string imageName)
        {
            Image look;
            if (!ImageExists(setId, imageName, out look)) throw new Exception("Image doesn't exist");

            return fileLocation + Path.DirectorySeparatorChar + data.sets[setLookUp[setId]].Path + Path.DirectorySeparatorChar + imageName;
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
            if (data.sets[setLookUp[setId]].images.Count <= 0) throw new NullReferenceException("No Images in set");

            foreach (Image img in data.sets[setLookUp[setId]].images) if (img.Sent == false) return false;
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

            foreach (Image img in data.sets[setLookUp[setId]].images)
            {
                if (img.File == imageName) return true;
            }

            return false;
        }

        private bool ImageExists(int setId, string imageName, out Image result)
        {
            if (!ImageSetExists(setId)) throw new KeyNotFoundException("Image Set not found");

            foreach (Image img in data.sets[setLookUp[setId]].images)
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

            return data.sets[setLookUp[setId]].images.Count;
        }

        public void Save()
        {
            using(StreamWriter stream = new StreamWriter(fileLocation))
            {
                stream.WriteLine(JsonConvert.SerializeObject(data));
            }
        }

        private void Load(string path)
        {
            using (StreamReader stream = new StreamReader(fileLocation))
            {
                string saved = stream.ReadToEnd();
                data = JsonConvert.DeserializeObject<Data>(saved);
            }

            for(int i = 0; i < data.sets.Count; i++)
            {
                setLookUp.Add(data.sets[i].ImageSetId,i);
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
                    foreach (Image img in images)
                        if (!img.Sent) return false;
                    return true;
                }
            }

            [JsonProperty(PropertyName = "images")]
            public List<Image> images = new List<Image>();
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

        public class Camera
        {
            [JsonProperty(PropertyName = "name")]
            public string CameraName { get; set; }
            [JsonProperty(PropertyName = "id")]
            public int CameraId { get; set; }
        }
        #endregion
    }
}
