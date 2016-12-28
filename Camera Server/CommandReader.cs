﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SharedDeviceItems;

namespace Camera_Server
{
    /// <summary>
    /// Abstracts the extraction of prameters and camera request from the raw byte data
    /// </summary>
    public class CommandReader
    {
        public Dictionary<string, string> Parameters { get; private set; }
        public CameraRequest Request { get; private set; }

        public CommandReader(byte[] data)
        {
            Parameters = new Dictionary<string, string>();
            string[] listed = Regex.Split(Encoding.ASCII.GetString(StripToBasicMessage(data)), Constants.ParamSeperator);

            CameraRequest pre;
            CameraRequest.TryParse(listed[0], out pre);
            Request = pre;

            for (int i = 1; i < listed.Length; i++)
            {
                int sepindex = listed[i].IndexOf(Constants.ParamKeyValueSeperator);
                string key = listed[i].Substring(0, sepindex);
                string value = listed[i].Substring(sepindex + 1, listed[i].Length - sepindex - 1);

                Parameters.Add(key, value);
            }
        }

        private byte[] StripToBasicMessage(byte[] data)
        {
            byte[] check = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            for (int i = 0; i <= data.Length - check.Length; i++)
            {
                if (data[i] == check[0])
                {
                    for (int j = 1; j < check.Length; j++)
                    {
                        if (data[i + j] == check[j])
                        {
                            if (j == check.Length - 1)
                            {
                                byte[] putput = new byte[i];
                                Array.Copy(data, putput, putput.Length);
                                return putput;
                            }
                        }
                        else break;
                    }
                }
            }

            return data;
        }
    }
}