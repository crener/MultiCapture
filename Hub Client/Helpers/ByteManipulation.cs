using System;
using System.Linq;
using System.Text;
using SharedDeviceItems;

namespace Hub.Helpers
{
    public static class ByteManipulation
    {
        /// <summary>
        /// Seperated the first section of data that matches the seperator string
        /// </summary>
        /// <param name="name">String value of the data before the seperator</param>
        /// <param name="rawData">Raw byte data that is parsed</param>
        /// <param name="imageData">bytes that are after the seperator</param>
        /// <returns>true if the operation was sucessful</returns>
        public static bool SeperateData(out string name, byte[] rawData, out byte[] imageData)
        {
            //initialise the name to blank encase there is no name in the data
            name = "";
            imageData = new byte[0];

            byte[] eom = Encoding.ASCII.GetBytes(Constants.MessageSeperator);
            bool foundSeperator = false;

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
                            foundSeperator = true;
                        }
                        else
                        {
                            foundSeperator = false;
                            break;
                        }
                    }

                    if (foundSeperator)
                    {
                        //looks like we found a match
                        name = Encoding.ASCII.GetString(rawData, 0, i);
                        imageData = new byte[rawData.Length - (i + Constants.MessageSeperator.Length)];
                        Array.Copy(rawData, (i + Constants.MessageSeperator.Length), imageData, 0, imageData.Length);

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
        public static bool SearchEndOfMessage(byte[] data, int size)
        {
            byte[] mesg = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            for (int i = size - 1; i >= 0; i--)
            {
                if (data[i] == mesg.Last())
                {
                    int i2 = i;
                    //last element has been found search for the lest of them
                    for (int u = mesg.Length - 1; u >= 0; u--, i2--)
                    {
                        if(u == 0 && mesg[u] == data[i2])
                        {
                            return true;
                        }
                        if (mesg[u] != data[i2])
                        {
                            break;
                        }
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
        /// <returns>first byte location of the end of message</returns>
        public static int SearchEndOfMessageIndex(byte[] data, int size)
        {
            byte[] mesg = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            for (int i = size - 1; i >= 0; i--)
            {
                if (data[i] == mesg.Last())
                {
                    int i2 = i;
                    bool valid = true;
                    //last element has been found search for the lest of them
                    for (int u = mesg.Length - 1; u >= 0; u--, i2--)
                    {
                        if (mesg[u] != data[i2])
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid) return i2;
                }
            }

            return -1;
        }
    }
}
