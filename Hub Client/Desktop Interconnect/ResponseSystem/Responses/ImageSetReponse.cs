using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Util;
using Newtonsoft.Json;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.ImageSetMetaData), ResponseType(ScannerCommands.ImageSetImageData)]
    internal class ImageSetReponse : BaseResponse
    {
        protected Dictionary<int, ProjectMapper> projectCache = new Dictionary<int, ProjectMapper>();

        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            //check that all parameters are met
            if (!parameters.ContainsKey("id"))
            {
                Console.WriteLine(command + "is missing parameter: id");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"id\" parameter is missing");
            }

            if (!parameters.ContainsKey("set"))
            {
                Console.WriteLine(command + "is missing parameter: set");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"set\" parameter is missing");
            }

            int project, set;
            bool success = int.TryParse(parameters["id"], out project);

            //check that the parameters are valid
            if (!success)
            {
                Console.WriteLine(command + " Failed to convert project id to a valid number");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"id\" could not be interprited. Is it possible the value is not a number?");
            }

            success = int.TryParse(parameters["set"], out set);
            if (!success)
            {
                Console.WriteLine(command + " Failed to convert image-set id to a valid number");
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"set\" could not be interprited. Is it possible the value is not a number?");
            }

            //find the project
            if (!projectCache.ContainsKey(project))
            {
                if (!FindProject(project))
                {
                    Console.WriteLine(command + " project couldn't be found");
                    return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?Project could not be found");
                }
            }

            //get the actual data
            if (command == ScannerCommands.ImageSetMetaData)
            {
                return GenerateImageMetaData(project, set);
            }
            else if (command == ScannerCommands.ImageSetImageData)
            {
                if (!parameters.ContainsKey("image"))
                {
                    Console.WriteLine(command + "is missing parameter: image");
                    return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"image\" parameter is missing");
                }

                int image;
                success = int.TryParse(parameters["image"], out image);

                if (!success)
                {
                    Console.WriteLine(command + " Failed to convert image id to a valid number");
                    return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"image\" could not be interprited. Is it possible the value is not a number?");
                }

                return GenerateImageData(project, set, image);
            }

            throw new UnknownResponseException();
        }

        private byte[] GenerateImageMetaData(int project, int imageSetNo)
        {
            ProjectMapper.ImageSet set = projectCache[project].saveData.sets.First(p => p.ImageSetId == imageSetNo);
            ImageSet output = new ImageSet(set);

            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(output));
        }

        private byte[] GenerateImageData(int project, int imageSetNo, int image)
        {
            ProjectMapper.ImageSet imageSet = projectCache[project].saveData.sets.First(p => p.ImageSetId == imageSetNo);
            try
            {
                string name = imageSet.Images.First(i => i.CameraId == image).File;
                string path = projectCache[project].AbsoluteImagePath(imageSetNo, name);

                projectCache[project].Sent(imageSetNo, name);
                return File.ReadAllBytes(path);
            }
            catch (IOException)
            {
                Console.WriteLine(ScannerCommands.ImageSetImageData + " attempted to access an iamge that doesn't exist!");
#if DEBUG
                Console.WriteLine("\tProject: " + project);
                Console.WriteLine("\tImageSet: " + imageSetNo);
                Console.WriteLine("\tImage: " + image);
#endif
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?image could not be found! image: " + image);
            }
        }

        /// <summary>
        /// loads project and adds it to the project cache
        /// </summary>
        /// <param name="projectId">the id of the requred project</param>
        /// <returns>true if the project could be loaded successfully</returns>
        protected virtual bool FindProject(int projectId)
        {
            //check if the current project is wanted
            if (Deployer.Manager != null && Deployer.Manager.ProjectId == projectId)
                projectCache.Add(projectId, Deployer.Manager.ProjectData);

            //get the project via a more conventional method
            if (!Deployer.ProjectManager.ProjectExists(projectId)) return false;

            string path = SharedDeviceItems.Constants.DefaultHubSaveLocation() + projectId;
            if (!Directory.Exists(path)) return false;

            ProjectMapper wanted = new ProjectMapper(path, -1);
            if (wanted.saveData.ProjectId != projectId) return false;

            projectCache.Add(projectId, wanted);

            return true;
        }

        public override void Reset()
        {
            projectCache.Clear();
        }

        private class ImageSet
        {
            public int id { get; set; }
            public List<Image> Images { get; set; }

            public ImageSet()
            {
                Images = new List<Image>();
            }

            public ImageSet(ProjectMapper.ImageSet original)
            {
                id = original.ImageSetId;
                Images = new List<Image>(original.Images.Count);

                foreach (ProjectMapper.Image image in original.Images)
                {
                    Images.Add(new Image
                    {
                        Id = image.CameraId,
                        Name = image.File
                    });
                }
            }
        }

        private struct Image
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}
