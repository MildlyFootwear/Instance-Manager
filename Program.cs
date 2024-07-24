using Instance_Manager.Properties;
using static Instance_Manager.CommonMethods;
using static Instance_Manager.CommonVars;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;

namespace Instance_Manager
{
    internal static class Program
    {
        
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            
            ApplicationConfiguration.Initialize();

            SetDriveVariables();
            Console.WriteLine("Loading environment variables...");

            int index = 0;

            foreach(string s in SystemVariables)
            {
                Console.WriteLine(s+" is " + SystemVariablesValues[index]);
                index++;
            }

            Console.WriteLine("");

            string usvfsdll = envEXELOC + "\\usvfs\\usvfs_x64.dll";
            
            if (!File.Exists(usvfsdll))
            {
                MessageBox.Show("Can't find "+usvfsdll,"Instance Manager - Fatal Error");
                return;
            }

            string VFSLauncher = envEXELOC + "\\usvfs\\VFSLauncher.exe";

            if (!File.Exists(VFSLauncher))
            {
                MessageBox.Show("Can't find "+VFSLauncher, "Instance Manager - Fatal Error");
                return;
            }

            if (!Settings.Default.CustomProfilesDirectory || Settings.Default.ProfilesDirectory == "NotInitialized")
            {
                Settings.Default.ProfilesDirectory = (envEXELOC) + "\\Profiles";
                Settings.Default.Save();
                Console.WriteLine("Updated profiles directory to "+ Settings.Default.ProfilesDirectory);
            }

            if (Settings.Default.ActiveProfile == "NotInitialized")
            {
                Settings.Default.ActiveProfile = "Profile1";
                Settings.Default.Save();
                Console.WriteLine("Initialized active profile.");
            }

            LoadProfiles();

            if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile))
                Settings.Default.ActiveProfile = Profiles[0];

            LoadProfileExes();

            Form ProfileM = new ProfileManager();
            Application.Run(new MainUI());
        }
    }
}