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
            // To customize application configuration such as set high DPI settings or default font,
            
            ApplicationConfiguration.Initialize();

            ImmutableList<string> argsL = args.ToImmutableList();

            if (argsL.IndexOf("-debug") != -1)
                Debug = true;

            if (Debug)
                AllocConsole();

            if (argsL.IndexOf("-quicklaunch") != -1)
                QuickLaunch = true;

            static void ExceptionHandler(object sender, EventArgs ex)
            {
                MessageBox.Show(ex.ToString(), ToolName+" - Exception");
            }

            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

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
                if (MessageBox.Show("Can't find "+usvfsdll+"\nPress OK to open up a link to the github repository where you can download the required files.", ToolName + " - Fatal Error") == DialogResult.OK)
                    System.Diagnostics.Process.Start("explorer.exe", "https://github.com/MildlyFootwear/Instance-Manager");
                
                return;
            }

            string VFSLauncher = envEXELOC + "\\usvfs\\VFSLauncher.exe";

            if (!File.Exists(VFSLauncher))
            {
                if (MessageBox.Show("Can't find "+VFSLauncher + "\nPress OK to open up a link to the github repository where you can download the required files.", ToolName + " - Fatal Error") == DialogResult.OK)
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
                    } else { MessageBox.Show("No executables to show for " + Settings.Default.ActiveProfile); }
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
                string ver = "";
                Console.WriteLine("Checking for update");
                bool CheckedForUpdate = true;
                try
                {
                    Stream stream = client.OpenRead("https://raw.githubusercontent.com/MildlyFootwear/Instance-Manager/master/ver.txt");
                    StreamReader reader = new StreamReader(stream);
                    ver = reader.ReadToEnd();
                    Console.WriteLine(ver);
                    reader.Close();
                    stream.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine (ex.Message);
                    CheckedForUpdate = false;
                }

                if (CheckedForUpdate)
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
                            } else if (result == 3)
                            {
                                Settings.Default.IngoreVersion = ver;
                                Settings.Default.Save();
                                Console.WriteLine("Ignoring version "+ver);
                            }
                        }
                    }
                    if (ver == Settings.Default.IngoreVersion) 
                        { Console.WriteLine(ver + " is ignored"); }
                    if (ver == Settings.Default.Version)
                        Console.WriteLine(Settings.Default.Version+" is up to date with repository version "+ver);

                }

                if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile))
                    Settings.Default.ActiveProfile = Profiles[0];

                LoadProfileExes();
                Application.Run(new MainUI());
            }

            if (Debug) { Console.WriteLine("\nProgram end reached"); Thread.Sleep(10000); }

        }
    }
}