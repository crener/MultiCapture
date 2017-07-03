
using System.Text;

namespace Hub.DesktopInterconnect
{

    public class ResponseConstants
    {
        public const float ApiVersion = 1f;

        public const string SuccessString = "Success";
        public const string FailString = "Fail";

        public static readonly byte[] SuccessResponse = Encoding.ASCII.GetBytes(SuccessString);
        public static readonly byte[] ApiResponse = Encoding.ASCII.GetBytes(ApiVersion.ToString("F"));
    }


    /// <summary>
    /// Scanner command response codes
    /// </summary>
    public enum ScannerCommands
    {
        Unknown = 0,

        //Global Commands
        setName = 100,
        getRecentLogFile = 120,
        getRecentLogDiff = 121,
        getLoadedProjects = 130,
        getCameraConfiguration = 140,
        getCapacity = 150,
        getApiVersion = 180,

        //Camera Commands
        CaptureImageSet = 200,

        //Project Management Commands
        RemoveProject = 300,
        getAllImageSets = 310,
        getImageSet = 320,
        getProjectStats = 330,
        setProjectNiceName = 110,
    }
}
