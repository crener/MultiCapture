using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Hub.DesktopInterconnect;
using Hub.Util;
using Newtonsoft.Json;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.ImageSetMetaData), ResponseType(ScannerCommands.ImageSetImageData)]
    internal class ImageSetReponse : BaseResponse
    {
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
            if (!FindProject(project))
            {
                    Console.WriteLine(command + " project couldn't be found");
                    return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?Project could not be found");
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

        protected virtual bool FindProject(int projectId)
        {
            try
            {
                ProjectCache.RetrieveProject(projectId);
                return true;
            }
            catch(KeyNotFoundException)
            {
                return false;
            }
        }

        private byte[] GenerateImageMetaData(int project, int imageSetNo)
        {
            ProjectMapper.ImageSet set;
            try
            {
                set = ProjectCache.RetrieveProject(project).saveData.sets.First(p => p.ImageSetId == imageSetNo);
            }
            catch (KeyNotFoundException)
            {
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?image could not be found! imageSet: " + imageSetNo);
            }

            ImageSet output = new ImageSet(set);

            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(output));
        }

        private byte[] GenerateImageData(int project, int imageSetNo, int image)
        {
            ProjectMapper projectMapper;
            ProjectMapper.ImageSet imageSet;
            try
            {
                projectMapper = ProjectCache.RetrieveProject(project);
                imageSet = projectMapper.saveData.sets.First(p => p.ImageSetId == imageSetNo);
            }
            catch(KeyNotFoundException)
            {
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?image could not be found! image: " + image);
            }

            try
            {
                string name = imageSet.Images.First(i => i.CameraId == image).File;
                string path = projectMapper.AbsoluteImagePath(imageSetNo, name);

                projectMapper.Sent(imageSetNo, name);
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
                return Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?image could not be accessed! image: " + image);
            }
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
