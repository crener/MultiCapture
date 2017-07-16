using System.Collections.Generic;
using Hub.DesktopInterconnect;

namespace Hub.ResponseSystem
{
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
    }
}
