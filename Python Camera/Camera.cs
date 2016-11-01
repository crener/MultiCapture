using System;
using System.Collections.Generic;
using System.Threading;
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting.Hosting;
using SharedDeviceItems.Interface;

namespace PythonCamera
{
    public class Camera : ICamera
    {
        private ScriptEngine engine;
        private ScriptScope script;
        private ObjectOperations ops;
        private dynamic pyCam;

        //thread control parameters
        private bool captureImage,
            shutdown = false,
            changeSetting;
        private string imageName;

        //camera settings
        private string fileLocation;
        private string camName;
        private int x = 3280, y = 2464;

        public Camera(string name, string saveLocation = "/scanImage/")
        {
            try
            {
                //ScriptRuntime run = IronPython.Hosting.Python.CreateRuntime();
                engine = Python.CreateEngine();
                ICollection<string> paths = engine.GetSearchPaths();
                script = engine.CreateScope();
                ops = engine.Operations;
                
                paths.Add("/usr/lib/python2.7");
                paths.Add("/usr/lib/python2.7/lib-old");
                paths.Add("/usr/lib/pymodules/python2.7");
                paths.Add("/usr/lib/python2.7/dist-packages");
                paths.Add("/usr/local/lib/python2.7/dist-packages");
                paths.Add("using/");
                engine.SetSearchPaths(paths);

                pyCam = engine.Runtime.UseFile("Python.py");

                var type = script.GetVariable("python");
                pyCam = ops.CreateInstance(type);

                camName = name;
                var newName = pyCam.setCamName("23");
                if (newName != "23") throw new InvalidOperationException("python name failed");

                fileLocation = saveLocation;
                var newLocation = pyCam.changeLocation(saveLocation);
                if (newLocation != saveLocation) throw new InvalidOperationException("python location failed");

                pyCam.setResulution(x, y);

                Thread maintanceThread = new Thread(MaintainCamera);
                if (maintanceThread.IsAlive) maintanceThread.Start();
            }
            catch (ImportException e)
            {
                Console.WriteLine("Error Finding Package: " + e.Message);
                Console.WriteLine("Camera Module shutting down!");
                shutdown = true;
            }
            catch (TypeInitializationException e)
            {
                Console.WriteLine("Error Instantiating: " + e.Message);
                Console.WriteLine("Camera Module shutting down!");
                shutdown = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown error: " + e.Message);
                shutdown = true;
            }
        }

        public void SetDirectory(string location)
        {
            fileLocation = location;
            changeSetting = true;
        }

        public void SetCameraName(string name)
        {
            camName = name;
            changeSetting = true;
        }

        public string CaptureImage(string identifier)
        {
            imageName = identifier;
            captureImage = true;

            //wait for image to be taken
            while (captureImage) Thread.Sleep(50);

            return imageName;
        }

        public void SetResulution(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// since the python libary needs to keep the camera "alive" to keep it focussed with correct 
        /// light livels, and so on. This will keep the python busy while the camera waits for commands
        /// </summary>
        private void MaintainCamera()
        {
            while (!shutdown)
            {
                if (captureImage)
                {
                    imageName = pyCam.captureAutomatic(imageName);
                    captureImage = false;
                }
                else if (changeSetting)
                {
                    var newName = pyCam.setCamName(camName);
                    if (newName != camName) throw new InvalidOperationException("python name failed");
 
                    var newLocation = pyCam.changeLocation(fileLocation);
                    if (newLocation != fileLocation) throw new InvalidOperationException("python location failed");

                    pyCam.setResulution(x, y);

                    changeSetting = false;
                }
                else pyCam.sleepCamera(0.05);
            }
        }
    }
}
