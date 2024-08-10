using Instance_Manager.Properties;
using static Instance_Manager.Methods.CommonMethods;
using static Instance_Manager.CommonVars;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Collections.Immutable;
using Instance_Manager.UtilityForms;
using System.Net;

namespace Instance_Manager
{
    internal static class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {
            
            ApplicationConfiguration.Initialize();

            static void ExceptionHandler(object sender, EventArgs ex)
            {
                MessageBox.Show(ex.ToString(), ToolName + " - Exception");
            }

            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

            ImmutableList<string> argsL = args.ToImmutableList();

            if (argsL.IndexOf("-debug") != -1)
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
                Console.Write("Passed args: ");
                foreach (string arg in argsL)
                {
                    Console.Write(arg + "  ");
                }
                Console.WriteLine();
            }

            if (argsL.IndexOf("-quicklaunch") != -1)
                QuickLaunch = true;


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

            string VFSLauncher = envEXELOC + "\\usvfs\\VFSLauncher.exe";

            if (!File.Exists(VFSLauncher))
            {
                if (MessageBox.Show("Can't find "+VFSLauncher + "\nOpen the github repository so you can download the required files?", ToolName + " - Fatal Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                Settings.Default.ActiveProfile = "Default1";
                Settings.Default.Save();
                Console.WriteLine("Initialized active profile.");
            }

            LoadProfiles();

            if (QuickLaunch)
            {

                Console.WriteLine("\nQuick launching\n");

                string passedexe = null;
                string passedprofile = null;

                foreach (string arg in args)
                {
                    Console.WriteLine(arg);
                    if (Path.GetExtension(arg) == ".exe")
                    {
                        if (File.Exists(arg))
                        {
                            passedexe = arg;
                            Console.WriteLine("Found exe "+arg+" in args.");
                            SelectedExe = arg;
                        }
                        
                    } else if (Directory.Exists(Settings.Default.ProfilesDirectory+"\\"+arg))
                    {
                        passedprofile=arg;
                        Console.WriteLine("Using profile "+arg);
                    }
                }

                if (passedprofile == null)
                {
                    Console.WriteLine("\nShowing quick profile");
                    Form QuickProf = new QuickProfile();
                    QuickProf.ShowDialog();
                }

                LoadProfileLinks();

                if (passedexe == null)
                {
                    LoadProfileExes();
                    if (ProfileExes.Count > 0)
                    {
                        Form QuickExe = new QuickExe();
                        QuickExe.ShowDialog();
                    } else { MessageBox.Show("No executables found for " + Settings.Default.ActiveProfile);return; }
                } else
                {
                    index = argsL.IndexOf(SelectedExe) + 1;
                    SelectedExe += ";";
                    while (index < argsL.Count)
                    {
                        SelectedExe += " "+argsL[index];
                        index++;
                    }
                }

                if (QuickLaunch)
                LaunchExe();

            } else
            {

                WebClient client = new();
                string ver = CheckGitVersion();
                if (ver != null)
                {

                    if (ver != Settings.Default.Version && ver != Settings.Default.IngoreVersion)
                    {
                        var taskDialog = TaskDialog.ShowDialog(new TaskDialogPage
                        {
                            Caption = ToolName,
                            Text = "Update is available. Go to download page?",
                            Buttons =
                            {
                                new TaskDialogButton
                                {
                                    Text = "Yes",
                                    Tag = 1
                                },
                                new TaskDialogButton
                                {
                                    Text = "No",
                                    Tag = 2
                                },
                                new TaskDialogButton
                                {
                                    Text = "Ignore this version",
                                    Tag = 3
                                }

                            }
                        });
                        if (taskDialog.Tag is int result)
                        {
                            if (result == 1)
                            {
                                System.Diagnostics.Process.Start("explorer.exe", "https://github.com/MildlyFootwear/Instance-Manager");
                                return;
                            }
                            else if (result == 3)
                            {
                                Settings.Default.IngoreVersion = ver;
                                Settings.Default.Save();
                                Console.WriteLine("Ignoring version " + ver);
                            }
                        }
                    }
                    if (ver == Settings.Default.IngoreVersion)
                    { Console.WriteLine(ver + " is ignored"); }
                    else if (ver == Settings.Default.Version)
                    {
                        Console.WriteLine(Settings.Default.Version + " is up to date with repository version " + ver);

                    }
                }

                if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile))
                {
                    Settings.Default.ActiveProfile = Profiles[0];
                }

                LoadProfileExes();
                Application.Run(new MainUI());
            }

            if (Debug) { Console.WriteLine("\nProgram end reached"); Thread.Sleep(2000); }

        }
    }
}