using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Hub.Helpers;
using SharedDeviceItems;

namespace Hub
{
    class Capture
    {
        private static Dictionary<int, Thread> threads = new Dictionary<int, Thread>();
        private static SaveContainer.Data config;

        public Capture(SaveContainer.Data configuration)
        {
            config = configuration;
        }

        public void InitiateCaptureImage(CameraRequest action, Socket[] connections)
        {
            foreach (Socket connection in connections)
            {
                ThreadPool.QueueUserWorkItem(state => CaptureImage(action, connection));
            }
        }

        private void CaptureImage(CameraRequest action, Socket connection)
        {
            Console.WriteLine("Thread Active" + action + ", Socket " + connection.LocalEndPoint);
        }
    }
}