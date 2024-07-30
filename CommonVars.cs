using Instance_Manager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instance_Manager
{
    public class CommonVars
    {

        public static string ToolName = "Instance Manager";

        public static List<string> Profiles = ["Empty"];
        public static List<string> DirectoryLinks = [];
        public static List<string> ProfileExes = [];

        public static bool QuickLaunch = false;
        public static bool Debug = false;

        public static string SelectedExe = "";
        public static string ProfilePath = "";
        public static string TextInputString = "";
        public static string PassedString = "";
        public static bool NeedRefresh = false;


        public static string envUSERPROFILE = Environment.GetEnvironmentVariable("USERPROFILE");
        public static string envEXELOC = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static List<string> SystemVariablesValues = new List<string> { envEXELOC, envUSERPROFILE  };
        public static List<string> SystemVariables = new List<string> { "%APPLOC%", "%USERPROFILE%" };
        
    }
}
