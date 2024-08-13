using Instance_Manager.Properties;
using static Instance_Manager.Methods.CommonMethods;
using static Instance_Manager.Methods.UpdateMethods;
using static Instance_Manager.CommonVars;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Collections.Immutable;
using Instance_Manager.UtilityForms;
using System.Net;
using XInput.Wrapper;

namespace Instance_Manager
{
    internal static class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {
            
            ApplicationConfiguration.Initialize();

            static void ExceptionHandler(object sender, UnhandledExceptionEventArgs ex)
            {
                MessageBox.Show((ex.ExceptionObject).ToString(), ToolName + " - Exception");
            }

            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

            ImmutableList<string> argsL = args.ToImmutableList();

            if (argsL.IndexOf("-debug") == 0 || argsL.IndexOf("-debug") == 1)
                Debug = true;

            if (Debug)
                AllocConsole();

            Console.WriteLine("Starting Instance Manager.");

            if (argsL.Count == 0)
            {
                Console.WriteLine("No args passed.");
            }
            else
            {
                Console.WriteLine("Passed args: ");
                foreach (string arg in argsL)
                {
                    Console.WriteLine(arg);
                }
                Console.WriteLine();
            }

            if (argsL.IndexOf("-quicklaunch") == 0 || argsL.IndexOf("-quicklaunch") == 1)
                QuickLaunch = true;

            if (X.IsAvailable)
            {
                gamepad = X.Gamepad_1;
            }

            int index = 0;

            SetDriveVariables();

            foreach (string s in SystemVariables)
            {
                Console.WriteLine(s+" is " + SystemVariablesValues[index]);
                index++;
            }

            Console.WriteLine("");

            string usvfsdll = envEXELOC + "\\usvfs\\usvfs_x64.dll";
            
            if (!File.Exists(usvfsdll))
            {
                if (MessageBox.Show("Can't find "+usvfsdll+"\nOpen the github repository so you can download the required files?", ToolName + " - Fatal Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    System.Diagnostics.Process.Start("explorer.exe", "https://github.com/MildlyFootwear/Instance-Manager");
                
                return;
            }

            string VFSLauncher = envEXELOC + "\\usvfs\\usvfsWrap.dll";

            if (!File.Exists(VFSLauncher))
            {
                if (MessageBox.Show("Can't find "+VFSLauncher + "\nOpen the github repository so you can download the required files?", ToolName + " - Fatal Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    return;
            }

            if (!Settings.Default.CustomProfilesDirectory || Settings.Default.ProfilesDirectory == "NotInitialized")
            {
                Settings.Default.ProfilesDirectory = envEXELOC + "\\Profiles";
                Settings.Default.Save();
                Console.WriteLine("Updated profiles directory to "+ Settings.Default.ProfilesDirectory);
            }

            if (Settings.Default.ActiveProfile == "NotInitialized")
            {
                Settings.Default.ActiveProfile = "Default1";
                Settings.Default.Save();
                Console.WriteLine("Initialized active profile.");
            }

            LoadProfiles();

            if (QuickLaunch)
            {

                QuickLaunchM.QuickLaunchMethod(argsL);

            } else
            {
                
                CheckForUpdate();

                if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile))
                {
                    Settings.Default.ActiveProfile = Profiles[0];
                }

                LoadProfileExes();
                Application.Run(new MainUI());

            }

            if (gamepad != null)
                X.StopPolling();
            if (VFSActive)
                Console.WriteLine();
            while (VFSActive && QuickLaunch == false)
            {
                Console.Write("\r"+DateTime.Now.ToString("HH:mm:ss")+" VFS still active with "+VFSHookedProcesses+" processes hooked into it.");
                Thread.Sleep(1000);
            }

            if (Debug) { Console.WriteLine("\nProgram end reached"); Thread.Sleep(4000); }

        }
    }
}