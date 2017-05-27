using System.Collections.Generic;

namespace Hub.DesktopInterconnect.Responses
{
    [ResponseType(ScannerCommands.getRecentLogFile),
     ResponseType(ScannerCommands.getRecentLogDiff)]
    class LogResponse : BaseResponse
    {
        private long lastLogPosition = -1;

        public override byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
        {
            if (command != ScannerCommands.getRecentLogDiff ||
                command != ScannerCommands.getRecentLogFile)
                throw new UnknownResponseException();

            return null;
        }

        public override void Reset()
        {
            lastLogPosition = -1;
        }
    }
}
