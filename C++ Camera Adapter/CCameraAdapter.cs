using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedDeviceItems.Interface;

namespace CCameraAdapter
{
    /// <summary>
    /// This is responsible for taking the direct output from the C++ Camera
    /// so that the output is handled correctly and can be passed on in native C#, 
    /// rather than making whatever wants the image being responsible for converting
    /// C++ data types to C# data types.
    /// </summary>
    public class CCamera : ICamera
    {
        public string CaptureImage(string identifier)
        {
            throw new NotImplementedException();
        }

        public byte[] CaptureImage(string identifier, bool name)
        {
            throw new NotImplementedException();
        }

        public byte[] CaptureImageByte(string identifier)
        {
            throw new NotImplementedException();
        }

        public void SetCameraName(string name)
        {
            throw new NotImplementedException();
        }

        public void SetDirectory(string location)
        {
            throw new NotImplementedException();
        }

        public void SetResolution(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
