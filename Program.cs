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
using Instance_Manager.Methods;

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

            List<string> argsForIM = new List<string>();

            foreach (string arg in args)
            {
                if (arg.IndexOf(".exe") != -1)
                    break;
                argsForIM.Add(arg);
            }

            if (argsForIM.IndexOf("-debug") != -1)
                ToolDebug = true;

            if (ToolDebug)
            { 
                AllocConsole(); 
            }

            WriteLineIfDebug("Starting Instance Manager.");

            if (argsL.Count == 0)
            {
                WriteLineIfDebug("No args passed.");
            }
            else
            {
                WriteLineIfDebug("Using args:");
                int ind = 0;
                foreach (string arg in argsForIM)
                {
                    WriteLineIfDebug("    "+arg);
                    ind++;
                }
                if (argsL.Count != argsForIM.Count)
                {
                    WriteLineIfDebug("Exe args:");
                    while (ind < argsL.Count)
                    {
                        WriteLineIfDebug("    " + argsL[ind]);
                        ind++;
                    }
                }
            }

            if (argsForIM.IndexOf("-quicklaunch") != -1)
                QuickLaunch = true;

            if (X.IsAvailable)
            {
                gamepad = X.Gamepad_1;
            }

            SetDriveVariables();

            string usvfsDll = envAPPLOC + "\\usvfs\\usvfs_x64.dll";
            
            if (!File.Exists(usvfsDll))
            {
                if (MessageBox.Show("Can't find "+usvfsDll+"\nOpen the github repository so you can download the required files?", ToolName + " - Fatal Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    System.Diagnostics.Process.Start("explorer.exe", "https://github.com/MildlyFootwear/Instance-Manager");
                
                return;
            }

            string usvfsWrapDll = envAPPLOC + "\\usvfs\\usvfsWrap.dll";

            if (!File.Exists(usvfsWrapDll))
            {
                if (MessageBox.Show("Can't find "+usvfsWrapDll + "\nOpen the github repository so you can download the required files?", ToolName + " - Fatal Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    return;
            }

            if (!Settings.Default.CustomProfilesDirectory || Settings.Default.ProfilesDirectory == "NotInitialized")
            {
                Settings.Default.ProfilesDirectory = envAPPLOC + "\\Profiles";
                Settings.Default.Save();
                WriteLineIfDebug("Updated profiles directory to "+ Settings.Default.ProfilesDirectory);
            }

            if (Settings.Default.ActiveProfile == "NotInitialized")
            {
                Settings.Default.ActiveProfile = "Default1";
                Settings.Default.Save();
                WriteLineIfDebug("Initialized active profile.");
            }

            LoadProfiles();

            if (QuickLaunch)
            {

                QuickLaunchM.QuickLaunchMethod(argsL);

            } else
            {

                UpdateMethods updateMethods = new UpdateMethods();

                updateMethods.CheckForUpdate();

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
                WriteLineIfDebug();

            while (VFSActive && QuickLaunch == false)
            {
                WriteIfDebug("\r"+DateTime.Now.ToString("HH:mm:ss")+" VFS still active with "+VFSHookedProcesses+" processes hooked into it.");
                Thread.Sleep(1000);
            }

            while (VFSActive)
                Thread.Sleep(1000);

            if (ToolDebug) { WriteLineIfDebug("\n"+ToolName+" end reached"); Thread.Sleep(4000); }

        }
    }
}