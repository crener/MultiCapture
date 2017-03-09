using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedDeviceItems;

namespace Hub.Helpers.Interface
{
    interface ICameraManager
    {
        string SavePath { get; set; }

        void CaptureImageSet();
        void CaptureImageSet(CameraRequest wanted);
        void ClearSockets();
    }
}
