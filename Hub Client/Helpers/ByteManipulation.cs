﻿using System;
using System.Text;
using SharedDeviceItems;

namespace Hub.Helpers
{
    public static class ByteManipulation
    {
        /// <summary>
        /// Separated the first section of data that matches the separator string
        /// </summary>
        /// <param before="before">String value of the data before the separator</param>
        /// <param before="rawData">Raw byte data that is parsed</param>
        /// <param before="after">bytes that are after the separator</param>
        /// <returns>true if the operation was successful</returns>
        public static bool SeparateData(out string before, byte[] rawData, out byte[] after, string seperator = Constants.MessageSeparator)
        {
            //initialize the before to blank encase there is no before in the data
            before = "";
            after = new byte[0];

            byte[] eom = Encoding.ASCII.GetBytes(seperator);
            bool foundSeparator = false;

            for (int i = 0; i < rawData.Length; i++)
            {
                //see if the first char matches
                if (eom[0] == rawData[i])
                {
                    //see if the rest of the message seperator can be found
                    for (int j = 1; j < eom.Length; j++)
                    {
                        if (eom[j] == rawData[i + j])
                        {
                            foundSeparator = true;
                        }
                        else
                        {
                            foundSeparator = false;
                            break;
                        }
                    }

                    if (foundSeparator)
                    {
                        //looks like we found a match
                        before = Encoding.ASCII.GetString(rawData, 0, i);
                        after = new byte[rawData.Length - (i + seperator.Length)];
                        Array.Copy(rawData, i + seperator.Length, after, 0, after.Length);

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the end of message string is inside the data 
        /// </summary>
        /// <param name="data">the data array to parse</param>
        /// <param name="size">amount of data populated with valid data (starting from 0)</param>
        /// <returns>true of the data contains the endof message string</returns>
        public static bool ContainsEom(byte[] data, int size)
        {
            return SharedDeviceItems.Helpers.ByteHelpers.SearchEomIndex(data, size) != -1;
        }
    }
}
