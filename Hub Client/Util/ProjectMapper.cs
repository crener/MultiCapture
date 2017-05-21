using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

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
        private static ProjectMapper instance;
        public int ProjectId { get; private set; }
        public string ProjectName { get; set; }

        private const int Version = 2;
        private int loadVersion = -1;
        private bool transferComplete = false;

        private List<ImageSet> sets = new List<ImageSet>();
        protected List<Camera> cameras = new List<Camera>();
        private Dictionary<int, int> setLookUp = new Dictionary<int, int>();

        public int ImageSetCount => sets.Count;
        public int CameraCount => cameras.Count;
        string fileLocation { get; set; }

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

        public ProjectMapper(string project, int projectId)
        {
            fileLocation = project;
            if (File.Exists(project)) Load(project);
            else ProjectId = projectId;

            if (instance == null) instance = this;
        }

        /// <summary>
        /// returns a static project mapper.
        /// Note the class using this is responsible for initializing this if it is null
        /// </summary>
        /// <returns>static project mapper</returns>
        public static ProjectMapper Instance
        {
            get
            {
                return instance;
            }
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

            sets[setLookUp[setId]].images.Add(new Image() { File = name, CameraId = camId });
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

            setLookUp.Add(setId, sets.Count);
            sets.Add(new ImageSet { ImageSetId = setId, Path = path });
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

            if (cameras.Contains(newCam)) throw new Exception("Camera already exists");

            cameras.Add(newCam);
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

            return fileLocation + Path.DirectorySeparatorChar + sets[setLookUp[setId]].Path + Path.DirectorySeparatorChar + imageName;
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
            return sets[setId].AllSent;
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
            if (sets[setLookUp[setId]].images.Count <= 0) throw new NullReferenceException("No Images in set");

            foreach (Image img in sets[setLookUp[setId]].images) if (img.Sent == false) return false;
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

            foreach (Image img in sets[setLookUp[setId]].images)
            {
                if (img.File == imageName) return true;
            }

            return false;
        }

        private bool ImageExists(int setId, string imageName, out Image result)
        {
            if (!ImageSetExists(setId)) throw new KeyNotFoundException("Image Set not found");

            foreach (Image img in sets[setLookUp[setId]].images)
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

            return sets[setLookUp[setId]].images.Count;
        }

        public void Save()
        {
            using (XmlWriter writer = XmlWriter.Create(fileLocation))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Project");
                writer.WriteElementString(MapperVersion, Version.ToString());
                writer.WriteElementString(ProdId, ProjectId.ToString());
                writer.WriteElementString(ProdName, ProjectName);
                writer.WriteElementString(Done, transferComplete.ToString());

                writer.WriteStartElement(CameraGroup);
                foreach (Camera cam in cameras)
                {
                    writer.WriteStartElement(CameraSetLabel);

                    writer.WriteAttributeString(CameraId, cam.CameraId.ToString());
                    writer.WriteAttributeString(CameraName, XmlConvert.EncodeName(cam.CameraName));

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement(ImageSetGroup);
                for (int i = 0; i <= sets.Count; ++i)
                {
                    if (!setLookUp.ContainsKey(i)) continue;
                    ImageSet set = sets[setLookUp[i]];

                    writer.WriteStartElement(XmlConvert.EncodeName(ImageSetLabel));

                    writer.WriteElementString(ImageSetId, set.ImageSetId.ToString());
                    writer.WriteElementString(ImageSetGroupFile, set.Path);
                    writer.WriteElementString(ImageSetGroupDone, set.AllSent.ToString());

                    writer.WriteStartElement(ImageGroupHeader);
                    foreach (Image img in set.images)
                    {
                        writer.WriteStartElement(ImageHeader);

                        writer.WriteAttributeString(ImagePath, img.File);
                        writer.WriteAttributeString(ImageCamera, img.CameraId.ToString());
                        writer.WriteAttributeString(ImageSent, img.Sent.ToString());
                        writer.WriteAttributeString(ImageSentDate, img.SendDate.ToString());

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void Load(string path)
        {
            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case ProdId:
                                int projID = -1;
                                if (reader.Read())
                                {
                                    if (int.TryParse(reader.Value, out projID)) ProjectId = projID;
                                    else Console.WriteLine("Project reader - Project ID could not be extracted");
                                }
                                break;
                            case ProdName:
                                ProjectName = reader.Value;
                                break;
                            case MapperVersion:
                                if (reader.Read()) int.TryParse(reader.Value, out loadVersion);

                                if (loadVersion != Version)
                                {
                                    Console.WriteLine("Project reader - Project Version doesn't match the current Version");
                                    Console.WriteLine("Project reader - Current Version:" + Version + ", Project Version: " + loadVersion);
                                }
                                break;
                            case Done:
                                bool doneVal;
                                if (reader.Read() && bool.TryParse(reader.Value, out doneVal)) transferComplete = doneVal;
                                break;
                            case ImageSetGroup:
                                while (reader.Read() && reader.Name == XmlConvert.DecodeName(ImageSetLabel))
                                    do
                                    {
                                        int setId = -1;
                                        if (reader.Read() && reader.Name == ImageSetId)
                                        {
                                            while (reader.NodeType != XmlNodeType.Text) reader.Read();
                                            int.TryParse(reader.Value, out setId);
                                        }
                                        while (reader.NodeType != XmlNodeType.EndElement) reader.Read();

                                        string setPath = null;
                                        if (reader.Read() && reader.Name == ImageSetGroupFile)
                                        {
                                            while (reader.NodeType != XmlNodeType.Text) reader.Read();
                                            setPath = reader.Value;
                                        }
                                        while (reader.NodeType != XmlNodeType.EndElement) reader.Read();

                                        AddImageSet(setId, setPath);

                                        //skip done
                                        reader.Read();
                                        while (reader.NodeType != XmlNodeType.EndElement) reader.Read();

                                        #region extract image data

                                        if (reader.Read() && reader.Name == ImageGroupHeader)
                                        {
                                            while (reader.Read() && reader.NodeType != XmlNodeType.EndElement &&
                                                  reader.Name != ImageGroupHeader)
                                            {
                                                if (reader.AttributeCount != 3)
                                                {
                                                    Console.WriteLine(
                                                        "Project reader - incorrect amount of information about image");
                                                    Console.WriteLine("Project reader - image attribute count: " +
                                                                      reader.AttributeCount + ", should be 3");
                                                }

                                                //image file name
                                                string file = reader.GetAttribute(ImagePath);
                                                int cam = int.Parse(reader.GetAttribute(ImageCamera));
                                                AddImage(setId, file, cam);

                                                //image sent?
                                                bool sentImg;
                                                bool.TryParse(reader.GetAttribute(ImageSent), out sentImg);

                                                //image send date
                                                if (sentImg)
                                                {
                                                    long sendDate = -1;
                                                    long.TryParse(reader.GetAttribute(ImageSentDate), out sendDate);
                                                    if (sendDate > 0) Sent(setId, file, sendDate);
                                                }
                                            }
                                        }

                                        #endregion

                                        while (reader.NodeType != XmlNodeType.Element && reader.Name != XmlConvert.DecodeName(ImageSetLabel)) reader.Read();
                                    } while (reader.Read() && reader.NodeType != XmlNodeType.EndElement && reader.Name != ImageSetGroup);
                                break;
                            case CameraGroup:
                                while (reader.Read() && reader.Name != XmlConvert.DecodeName(CameraSetLabel)) ;
                                do
                                {
                                    Camera newCam = new Camera
                                    {
                                        CameraId = int.Parse(reader.GetAttribute(CameraId)),
                                        CameraName = XmlConvert.DecodeName(reader.GetAttribute(CameraName))
                                    };

                                    cameras.Add(newCam);
                                } while (reader.Read() && reader.Name != CameraGroup);
                                break;
                            default:
                                if (reader.Name == "Project") break;
                                Console.WriteLine("Project reader - unknown state");
                                Console.WriteLine("Project reader - name: " + reader.Name);
                                Console.WriteLine("Project reader - value: " + reader.Value);
                                Console.WriteLine("Project reader - attributes: " + reader.AttributeCount);
                                break;
                        }
                    }
                }
            }
        }

        #region data containers
        private class ImageSet
        {
            public int ImageSetId { get; set; }
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

            public List<Image> images = new List<Image>();
        }

        private class Image
        {
            public string File { get; set; }
            public int CameraId { get; set; }
            public bool Sent { get; set; }
            public long SendDate { get; set; }
        }

        public class Camera
        {
            public string CameraName { get; set; }
            public int CameraId { get; set; }
        }
        #endregion
    }
}
