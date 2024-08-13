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

            usvfsWrapSetDebug(ToolDebug);

            VFSActive = usvfsWrapCreateVFS("test", false, LogLevel.Warning, CrashDumpsType.None, "", 200);

            if (VFSActive)
            {
                VFSInitializing = false;
                foreach (string s in ProfileDirectoryLinks)
                {
                    string[] link = ReplaceVariables(s).Split("|");
                    string source = link[0];
                    string destination = link[1];

                    usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_RECURSIVE);
                    usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_CREATETARGET);
                    usvfsWrapVirtualLinkDirectoryStatic(destination, source, LINKFLAG_MONITORCHANGES);

                }

                Thread.Sleep(1000);

                try
                {
                    // createFlags set to 0 as it is unneeded here.
                    usvfsWrapCreateProcessHooked(exe[1], exe[3], 0, exe[2]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                while (VFSHookedProcesses > 0) 
                    Thread.Sleep(1000);
                VFSActive = false;

            }
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
            if (ToolDebug)
                WriteLineIfDebug("VFS has been ended.");
        }
        Thread exeThread;

        void VFSHookedCountMonitor()
        {
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
        }
        Thread hookedCountMonitor;

        public void LaunchExe()
        {
            WriteLineIfDebug("\nExecuting Method: LaunchExe");
            WriteLineIfDebug("Attempting to launch " + SelectedExe);
            if (!VFSActive)
            {
                VFSInitializing = true;
                exeThread = new Thread(threadMethod);
                exeThread.Start();
                Thread.Sleep(200);
                hookedCountMonitor = new Thread(VFSHookedCountMonitor);
                hookedCountMonitor.Start();
                Thread.Sleep(100);
            }
            else
            {
                MessageBox.Show("VFS is already active with "+VFSHookedProcesses+" hooked processes.", ToolName);
            }
        }

    }

}
