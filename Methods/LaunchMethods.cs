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
        static int LastHookCount = 0;
        static void threadMethod()
        {
            string[] exe = ReplaceVariables(SelectedExe).Split("|");
            if (!File.Exists(exe[1]))
            {
                MessageBox.Show("Could not locate executable.\n" + exe[1], ToolName);
                return;
            }

            usvfsWrapSetDebug(Debug);

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
                usvfsWrapFree(); 
            } catch (Exception e) 
            {
                if (Debug)
                    Console.WriteLine(e.Message);


            }
            VFSInitializing = false;
            if (Debug)
                Console.WriteLine("VFS has been ended.");
        }
        static Thread exeThread;

        static void VFSHookedCountMonitor()
        {
            if (Debug)
                Console.WriteLine("HookMonitorThread started.");
            while (VFSInitializing)
            {
                Thread.Sleep(100);
            }
            while (VFSActive)
            {

                VFSHookedProcesses = usvfsWrapGetHookedCount();
                if ( VFSHookedProcesses != LastHookCount)
                {
                    LastHookCount = VFSHookedProcesses;
                    Console.WriteLine("HookMonitorThread: Updated VFSHookedProcesses to " + VFSHookedProcesses);
                }
                Thread.Sleep(1000);
            }
            if (Debug)
                Console.WriteLine("Hook monitor ended.");
        }
        static Thread hookedCountMonitor;

        public static void LaunchExe()
        {
            Console.WriteLine("\nExecuting Method: LaunchExe");
            Console.WriteLine("Attempting to launch " + SelectedExe);
            if (!VFSActive)
            {
                VFSInitializing = true;
                exeThread = new Thread(threadMethod);
                exeThread.Start();
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
