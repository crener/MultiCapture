using System;
using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;

namespace Hub.ResponseSystem
{
    /// <summary>
    /// Basic addition which registers the extending class with the Desktop Thread
    /// </summary>
    abstract class BaseResponse : IResponse
    {
        protected BaseResponse()
        {
            object[] responseTypes = GetType().GetCustomAttributes(typeof(ResponseTypeAttribute), false);

            foreach (ResponseTypeAttribute responseType in responseTypes)
            {
                if (!DesktopThread.Responders.ContainsKey(responseType.Response))
                    DesktopThread.Responders.Add(responseType.Response, this);
            }
        }

        public abstract byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters);

        public virtual void Reset()
        {

        }

        protected static bool hasParameter(ScannerCommands command, Dictionary<string, string> parameters, string wanted, out byte[] failMessage)
        {
            if (!parameters.ContainsKey(wanted))
            {
                Console.WriteLine(command + " missing \"" + wanted + "\" parameter");
                failMessage = Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?\"" + wanted + "\" parameter is missing");
                return false;
            }

            failMessage = null;
            return true;
        }

        protected static bool ExtractParameter(ScannerCommands command, Dictionary<string, string> parameters,
            string wanted, out string extractResult, out byte[] failMessage)
        {
            if (!hasParameter(command, parameters, wanted, out failMessage))
            {
                extractResult = null;
                return false;
            }

            extractResult = parameters[wanted];
            return true;
        }

        protected static bool ExtractParameter(ScannerCommands command, Dictionary<string, string> parameters,
            string wanted, out int extractResult, out byte[] failMessage)
        {
            if (!hasParameter(command, parameters, wanted, out failMessage))
            {
                extractResult = -1;
                return false;
            }

            bool success = int.TryParse(parameters[wanted], out extractResult);
            if(!success)
            {
                extractResult = -1;
                Console.WriteLine(command + " could't convert parameter: " + wanted);
                failMessage =
                    Encoding.ASCII.GetBytes(ResponseConstants.FailString + "?could not extract\"" + wanted + 
                    "\". Is \"" + parameters[wanted] + "\" valid?");
                return false;
            }

            return true;
        }
    }
}
