using Instance_Manager.Properties;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Instance_Manager.CommonVars;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using System.Runtime.ConstrainedExecution;
using System.Net;
using System.Security.Cryptography.Xml;
using static usvfsWrap.usvfsWrapM;
using System.Diagnostics;

namespace Instance_Manager.Methods
{

    public class LaunchMethods
    {
        int LastHookCount = 0;
        void threadMethod()
        {

            string[] exe = ReplaceVariables(SelectedExe).Split("|");
            if (!File.Exists(exe[1]))
            {
                MessageBox.Show("Could not locate executable.\n" + exe[1], ToolName);
                return;
            }

            if (ProfileDirectoryLinks.Count == 0)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(exe[1]);
                processStartInfo.WorkingDirectory = exe[2];
                processStartInfo.Arguments = exe[3];
                Process.Start(processStartInfo);
                return;
            }

            usvfsWrapSetDebug(ToolDebug);
            ActiveVFSName = Settings.Default.ActiveProfile;
            WriteLineIfDebug("    Initializing VFS "+ ActiveVFSName + " " + DateTimeOffset.Now.ToString("HHmmss"));
            VFSActive = usvfsWrapCreateVFS(ActiveVFSName + " " + DateTimeOffset.Now.ToString("HHmmss"), ToolDebug, LogLevel.Warning, CrashDumpsType.None, "", 200);

            if (VFSActive)
            {
                foreach (string s in ProfileDirectoryLinks)
                {

                    string[] link = ReplaceVariables(s).Split("|");
                    string source = Path.GetFullPath(link[0]);
                    string destination = Path.GetFullPath(link[1]);

                    usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_RECURSIVE);
                    usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_CREATETARGET);
                    usvfsWrapVirtualLinkDirectoryStatic(destination, source, LINKFLAG_MONITORCHANGES);

                }
                VFSInitializing = false;
                WriteLineIfDebug("    VFS initialized.");
                Thread.Sleep(1000);

                try
                {
                    // createFlags set to 0 as it is unneeded here.
                    usvfsWrapCreateProcessHooked(exe[1], exe[3], 0, exe[2]);
                }
                catch (Exception e)
                {
                    ThreadedMessage("Exception launching: " + SelectedExe.Split("|")[0]+"\n"+e.Message);
                }
                while (VFSHookedProcesses > 0) 
                    Thread.Sleep(1000);
                VFSActive = false;

            }
            while (VFSMonitoring)
                Thread.Sleep(100);
            try 
            {
                if (ToolDebug)
                    usvfsWrapCreateVFSDump();

                usvfsWrapFree(); 
            } catch (Exception e) 
            {
                if (ToolDebug)
                    WriteLineIfDebug(e.Message);
            }
            VFSInitializing = false;
            ActiveVFSName = "";

            if (ToolDebug)
                WriteLineIfDebug("VFS has been ended.");
        }
        static Thread exeThread;

        void VFSHookedCountMonitor()
        {
            VFSMonitoring = true;
            if (ToolDebug)
                WriteLineIfDebug("HookMonitorThread started.");
            while (VFSInitializing)
            {
                Thread.Sleep(100);
            }
            while (VFSActive)
            {
                Thread.Sleep(500);
                if (!VFSActive)
                    break;
                try
                {
                    VFSHookedProcesses = usvfsWrapGetHookedCount();
                }
                catch (Exception e)
                {
                    if (ToolDebug)
                        WriteLineIfDebug(e.Message);
                }
                if ( VFSHookedProcesses != LastHookCount)
                {
                    LastHookCount = VFSHookedProcesses;
                    WriteLineIfDebug("HookMonitorThread: Updated VFSHookedProcesses to " + VFSHookedProcesses);
                }
                Thread.Sleep(500);
            }
            if (ToolDebug)
                WriteLineIfDebug("Hook monitor ended.");
            VFSMonitoring = false;
        }
        static Thread hookedCountMonitor;

        public bool LaunchExe()
        {
            WriteLineIfDebug("\nExecuting Method: LaunchExe");
            WriteLineIfDebug("    Attempting to launch " + SelectedExe);
            if (!VFSActive)
            {
                VFSInitializing = true;
                exeThread = new Thread(threadMethod);
                exeThread.Start();
                Thread.Sleep(200);
                hookedCountMonitor = new Thread(VFSHookedCountMonitor);
                hookedCountMonitor.Start();

                if (!QuickLaunch)
                    ThreadedMessage("Launching " + SelectedExe.Split("|")[0], " - " + Settings.Default.ActiveProfile);

                return true;
            }
            else
            {
                ThreadedMessage("VFS for "+ActiveVFSName+" is already active with "+VFSHookedProcesses+" hooked processes.");
                return false;
            }
        }

    }

}
