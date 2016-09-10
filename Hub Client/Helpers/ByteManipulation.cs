using System;
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
    }
}
