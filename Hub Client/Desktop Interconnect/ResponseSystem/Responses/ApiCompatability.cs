using System.Collections.Generic;
using System.Text;
using Hub.DesktopInterconnect;
using Newtonsoft.Json;

namespace Hub.ResponseSystem.Responses
{
    [ResponseType(ScannerCommands.ApiCompatability)]
    class ApiCompatability : BaseResponse
    {
        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            Dictionary<int, string> avalibleResponses = new Dictionary<int, string>();
            Dictionary<int, string> sortedResponses = new Dictionary<int, string>();

            foreach (ScannerCommands commands in DesktopThread.Responders.Keys)
                avalibleResponses.Add((int)commands, commands.ToString());

            //sort the responses so that the lowest number is first
            List<int> sort = new List<int>(avalibleResponses.Keys);
            sort.Sort();
            foreach (int index in sort) sortedResponses.Add(index, avalibleResponses[index]);

            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(sortedResponses));
        }
    }
}
