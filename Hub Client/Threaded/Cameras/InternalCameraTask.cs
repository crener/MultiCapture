using System;
using System.IO;
using System.Threading.Tasks;
using Hub.Helpers;
using Hub.Util;
using SharedDeviceItems;
using SharedDeviceItems.Interface;
using Shell_Camera;

namespace Hub.Threaded.Cameras
{
    class InternalCameraTask : ICameraTask
    {
        ICamera cam;
        private SaveLoad.Data config;

        public InternalCameraTask(SaveLoad.Data config)
        {
            this.config = config;

            cam = new ShellCamera("Hub");
            cam.setFlip(config.internalCameraVFlip, config.internalCameraHFlip);
            cam.setRotation(config.internalCameraRotation);
            cam.SetResolution(config.internalCameraXRez, config.internalCameraYRez);
        }

        public void Dispose() { }

        public Task ProcessRequest(CameraRequest request)
        {
            return new Task(() => InternalRequest(request));
        }

        private void InternalRequest(CameraRequest request)
        {
            byte[] data;
            string tempFilePath = cam.CaptureImage(GenericManager.ImagesetId.ToString());

            if(File.Exists(tempFilePath))
            {
                data = File.ReadAllBytes(tempFilePath);
                File.Delete(tempFilePath);
            }
            else
            {
                Console.WriteLine("Camera " + config.internalCameraId + " failed capturing its image");
                return;
            }

            string imageName = "hub" + GenericManager.ImagesetId + ".jpg";
            string path = SavePath + Path.DirectorySeparatorChar + imageName;

            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.CreateNew))
                {
                    foreach (byte img in data)
                    {
                        fileStream.WriteByte(img);
                    }
                }
            }
            catch (IOException io)
            {
                Console.WriteLine("IO Exception saving file from Cam " + config.internalCameraId);
#if DEBUG
                Console.Write(io);
#endif
            }

            Deployer.CurrentProject.AddImage(GenericManager.ImagesetId, imageName, config.internalCameraId);
            Console.WriteLine("Camera " + config.internalCameraId + " image saved");
        }

        public string SavePath { get; set; }

#if DEBUG
        public void ClearSockets() { }
#endif
    }
}
