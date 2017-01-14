using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Hub.Util
{
    /// <summary>
    /// Responsible for maintaining the project file which keeps track of images and if they have been sent to a 
    /// computer for further processing.
    /// </summary>
    public class ImageMapper
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

        public int ProjectID { get; private set; }
        public string ProjectName { get; set; }

        private int Version = 1;
        private bool Done = false;

        List<ImageSet> sets = new List<ImageSet>();
        Dictionary<int, int> setLookUp = new Dictionary<int, int>();

        string FileLocation { get; set; }

        #region XML naming
        const string prodId = "projectID";
        const string prodName = "projectName";
        const string version = "versin";
        const string done = "done";

        const string cameraGroup = "cameras";
        const string cameraIp = "ip";
        const string cameraPort = "port";

        const string imageSetGroup = "imageSets";
        const string imageSetLabel = "set";
        const string imageSetGroupFile = "file";
        const string imageSetGroupDone = "done";
        const string imageSetId = "id";

        const string imageData = "images";
        const string image = "image";
        const string imagePath = "path";
        const string imageSent = "sent";
        const string imageSentDate = "sentDate";
        #endregion

        public ImageMapper(string project, int projectID)
        {
            FileLocation = project;
            if (File.Exists(project)) Load(project);
            else ProjectID = projectID;
        }

        /// <summary>
        /// Add a new image to an existing image set
        /// </summary>
        /// <param name="setID">Image set that the image gets added too</param>
        /// <param name="name">Name of the image</param>
        public void AddImage(int setID, string name)
        {
            if (ImageExists(setID, name)) throw new Exception("Image doesn't exist");

            sets[setLookUp[setID]].images.Add(new Image() { File = name });
        }

        /// <summary>
        /// adds a new image set
        /// </summary>
        /// <param name="setID">Image set that the image gets added too</param>
        /// <param name="path">the name of the folder containing all the images</param>
        public void AddImageSet(int setID, string path)
        {
            if (ImageSetExists(setID)) throw new KeyNotFoundException("Image Set already exists");

            setLookUp.Add(setID, sets.Count);
            sets.Add(new ImageSet() { ImageSetId = setID, Path = path });
        }

        public void Sent(int setID, string imageName)
        {
            Image look;
            if (!ImageExists(setID, imageName, out look)) throw new Exception("Image doesn't exist");

            look.Sent = true;
            look.SendDate = DateTime.Now.ToFileTimeUtc();
        }

        public string ImageAbsolutePath(int setID, string imageName)
        {
            Image look;
            if (!ImageExists(setID, imageName, out look)) throw new Exception("Image doesn't exist");

            return FileLocation + Path.DirectorySeparatorChar + sets[setLookUp[setID]].Path + imageName;
        }

        public bool hasSent(int setID, string imageName)
        {
            Image look;
            if (!ImageExists(setID, imageName, out look)) throw new Exception("Image doesn't exist");
            return look.Sent;
        }

        public long SendTime(int setID, string imageName)
        {
            Image look;
            if (!ImageExists(setID, imageName, out look)) throw new Exception("Image doesn't exist");
            return look.SendDate;
        }

        /// <summary>
        /// Returns true if all images in the image set have been sent
        /// </summary>
        /// <param name="setID">image set id</param>
        /// <returns>true if all images in the set have been sent</returns>
        public bool ImageSetIsDone(int setID)
        {
            if (!ImageSetExists(setID)) throw new KeyNotFoundException("Image Set not found");
            if (sets[setLookUp[setID]].images.Count <= 0) throw new NullReferenceException("No Images in set");

            foreach(Image img in sets[setLookUp[setID]].images) if (img.Sent == false) return false;
            return true; 
        }

        private bool ImageSetExists(int setID)
        {
            if (setLookUp.ContainsKey(setID)) return true;
            return false;
        }

        private bool ImageExists(int setID, string imageName)
        {
            if (!ImageSetExists(setID)) throw new KeyNotFoundException("Image Set not found");

            foreach (Image img in sets[setLookUp[setID]].images)
            {
                if (img.File == imageName) return true;
            }

            return false;
        }

        private bool ImageExists(int setID, string imageName,  out Image result)
        {
            if (!ImageSetExists(setID)) throw new KeyNotFoundException("Image Set not found");

            foreach (Image img in sets[setLookUp[setID]].images)
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

        public int ImageCount(int setID)
        {
            if (!ImageSetExists(setID)) throw new KeyNotFoundException("Image Set not found");

            return sets[setLookUp[setID]].images.Count;
        }

        public int ImageSetCount
        {
            get
            {
                return sets.Count;
            }
        }

        public void Save()
        {
            using (XmlWriter writer = XmlWriter.Create(FileLocation))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Project");
                writer.WriteElementString(prodId, ProjectID.ToString());
                writer.WriteElementString(prodName, ProjectName);
                writer.WriteElementString(version, Version.ToString());
                writer.WriteElementString(done, Done.ToString());

                writer.WriteStartElement(imageSetGroup);
                for (int i = 0; i < sets.Count; ++i)
                {
                    if (!setLookUp.ContainsKey(i)) continue;
                    ImageSet set = sets[setLookUp[i]];

                    writer.WriteStartElement(XmlConvert.EncodeName(imageSetLabel));

                    writer.WriteElementString(imageSetId, set.ImageSetId.ToString());
                    writer.WriteElementString(imageSetGroupFile, set.Path);
                    writer.WriteElementString(imageSetGroupDone, set.AllSent.ToString());

                    writer.WriteStartElement(imageData);
                    foreach (Image img in set.images)
                    {
                        writer.WriteStartElement(image);

                        writer.WriteAttributeString(imagePath, img.File);
                        writer.WriteAttributeString(imageSent, img.Sent.ToString());
                        writer.WriteAttributeString(imageSentDate, img.SendDate.ToString());

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
                            case prodId:
                                ProjectID = int.Parse(reader.Value);
                                break;
                            case prodName:
                                ProjectName = reader.Value;
                                break;
                            case version:
                                int ver = int.Parse(reader.Value);
                                if (ver != Version)
                                {
                                    Console.WriteLine("Project Version doesn't match the current Version");
                                    Console.WriteLine("Current Version:" + Version + ", Project Version: " + ver);
                                }
                                break;
                            case done:
                                Done = bool.Parse(reader.Value);
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
            public bool AllSent { get; set; }
            public List<Image> images = new List<Image>();
        }

        private class Image
        {
            public string File { get; set; }
            public bool Sent { get; set; }
            public long SendDate { get; set; }
        }
        #endregion
    }
}
