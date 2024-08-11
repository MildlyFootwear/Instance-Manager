using Instance_Manager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XInput.Wrapper;

namespace Instance_Manager
{
    public class CommonVars
    {

        public static string ToolName = "Instance Manager";
        public static string LatestVer = null;

        public static readonly HttpClient client = new HttpClient();


        public static List<string> Profiles = ["Empty"];
        public static List<string> DirectoryLinks = [];
        public static List<string> ProfileExes = [];

        public static bool QuickLaunch = false;
        public static bool Debug = false;
        public static X.Gamepad gamepad = null;

        public static string SelectedExe = "";
        public static string ProfilePath = "";
        public static string TextInputString = "";
        public static string PassedString = "";
        public static bool NeedSelectedProfileRefresh = false;
        public static bool NeedLinkMenuRefresh = false;


        public static string envUSERPROFILE = Environment.GetEnvironmentVariable("USERPROFILE");
        public static string envEXELOC = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string profPATH;
        public static List<string> SystemVariablesValues = new List<string> { "empty", envEXELOC, envUSERPROFILE };
        public static List<string> SystemVariables = new List<string> { "%ACTIVEPROFILE%", "%APPLOC%", "%USERPROFILE%" };
        
    }
}
