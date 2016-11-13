using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;
using SharedDeviceItems;

namespace Camera_Server
{
    /// <summary>
    /// Abstracts the extraction of prameters and camera request from the raw byte data
    /// </summary>
    public class CommandReader
    {
        public Dictionary<String, String> Parameters { get; private set; }
        public CameraRequest Request { get; private set; }

        public CommandReader(byte[] data)
        {
            Parameters = new Dictionary<string, string>();
            byte[] formattedData = new byte[data.Length - Constants.EndOfMessage.Length];
            Array.Copy(data, formattedData, formattedData.Length);

            string decoded = Encoding.ASCII.GetString(formattedData);
            String[] listed = Regex.Split(decoded, Constants.ParamSeperator);

            CameraRequest pre;
            CameraRequest.TryParse(listed[0], out pre);
            Request = pre;

            for(int i = 1; i < listed.Length; i++)
            {
                int sepindex = listed[i].IndexOf(Constants.ParamKeyValueSeperator);
                string key = listed[i].Substring(0, sepindex);
                string value = listed[i].Substring(sepindex + 1, listed[i].Length - sepindex - 1);

                Parameters.Add(key, value);
            }
        }
    }
}
