using Instance_Manager.Properties;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Instance_Manager.CommonVars;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.Runtime.ConstrainedExecution;
using System.Net;

namespace Instance_Manager.Methods
{
    public class CommonMethods
    {

        [DllImport("kernel32.dll")] public static extern bool AllocConsole();

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public static void SetDriveVariables()
        {
            Console.WriteLine("\nExecuting Method: SetDriveVariables");
            DriveInfo[] Drives = DriveInfo.GetDrives();
            foreach (DriveInfo d in Drives)
            {
                string label = d.VolumeLabel;
                if (label == null || label == "")
                {
                    label = "EMPTY";
                }
                string varLabel = "%DriveLabel " + label + "%";
                if (SystemVariables.IndexOf(varLabel) == -1)
                {
                    SystemVariables.Add(varLabel);
                    SystemVariablesValues.Add(d.Name.Substring(0, 2));
                }
                else
                {
                    MessageBox.Show("Variable " + varLabel + " is not free to assign for drive " + d.Name);
                }

            }
        }

        public static string InsertVariables(string path)
        {
            Console.WriteLine("\nExecuting Method: InsertVariables");
            int index = 0;
            foreach (string s in SystemVariablesValues)
            {
                //Console.WriteLine("Checking " + path + " for " + s + " to replace with" + SystemVariables[index]);
                path = path.Replace(s, SystemVariables[index]);
                index++;
            }
            Console.WriteLine("Returning " + path);
            return path;

        }

        public static string ReplaceVariables(string path)
        {
            Console.WriteLine("\nExecuting Method: ReplaceVariables");
            Console.WriteLine("Replacing variables for " + path);

            int index = 0;

            foreach (string s in SystemVariables)
            {
                //Console.WriteLine("Checking for " + s + " to replace with " + SystemVariablesValues[index]);
                path = path.Replace(s, SystemVariablesValues[index]);
                index++;
            }

            Console.WriteLine("Returning "+path);
            return path;
        }

        public static void LoadProfiles()
        {

            Console.WriteLine("\nExecuting Method: LoadProfiles");

            Profiles.Clear();
            if (!Directory.Exists(Settings.Default.ProfilesDirectory))
            {
                Directory.CreateDirectory(Settings.Default.ProfilesDirectory);
                Console.WriteLine("Created profiles directory.");
            }
            if (!Directory.EnumerateDirectories(Settings.Default.ProfilesDirectory).Any())
            {
                Directory.CreateDirectory(Settings.Default.ProfilesDirectory + "\\Default");
                Settings.Default.ActiveProfile = "Default";
                Settings.Default.Save();
                MessageBox.Show("No profiles found. Created default profile.", ToolName);
                Console.WriteLine("Created default profile.");
            }

            foreach (string prof in Directory.EnumerateDirectories(Settings.Default.ProfilesDirectory))
            {
                Profiles.Add(prof.Replace(Settings.Default.ProfilesDirectory + "\\", ""));
                Console.WriteLine("Added " + prof.Replace(Settings.Default.ProfilesDirectory + "\\", "") + " to profile list.");
            }

            string CurrentProfileList = "Profile options: ";
            foreach (string prof in Profiles)
            {
                CurrentProfileList += prof + ", ";
            }
            Console.WriteLine(CurrentProfileList.Substring(0, CurrentProfileList.Length - 2));

        }

        public static void SetProfile(string prof)
        {
            Console.WriteLine("\nExecuting Method: SetProfile");
            if (prof != "")
            {

                if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + prof))
                {
                    Directory.CreateDirectory(Settings.Default.ProfilesDirectory + "\\" + prof);
                    Thread.Sleep(100);
                    LoadProfiles();
                }

            }

            Settings.Default.ActiveProfile = prof;
            Settings.Default.Save();
            Console.WriteLine("Updated ActiveProfile to " + Settings.Default.ActiveProfile + ". Passed profile was " + prof + "\n");
            LoadProfileLinks();
            LoadProfileExes();

        }

        public static void LoadProfileLinks()
        {
            Console.WriteLine("\nExecuting Method: LoadProfileLinks");
            Console.WriteLine();
            profPATH = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile;
            SystemVariablesValues[SystemVariables.IndexOf("%ACTIVEPROFILE%")] = profPATH;
            string ProfileLinks = profPATH + "\\Links.txt";
            Console.WriteLine("Loading links for " + Settings.Default.ActiveProfile);
            DirectoryLinks.Clear();
            if (File.Exists(ProfileLinks))
            {
                var lines = File.ReadLines(ProfileLinks);
                foreach (var l in lines)
                {
                    string line = l;
                    if (line.IndexOf("|") == -1)
                        line = line.Replace(";", "|");
                    DirectoryLinks.Add(line);
                    string[] links = line.Split("|");
                    Console.WriteLine(line.Replace("|", " to "));
                    if (!Directory.Exists(ReplaceVariables(links[0])) && Settings.Default.SuppressMissingDirectory == false)
                    {
                        MessageBox.Show("Directory\n" + ReplaceVariables(links[0]) + "\nfor link\n" + line + "\nDoes not exist.", ToolName);
                    } else if (!Directory.Exists(ReplaceVariables(links[0])))
                        Console.WriteLine("Directory\n" + ReplaceVariables(links[0]) + "\nfor link\n" + line + "\nDoes not exist.");
                    if (!Directory.Exists(ReplaceVariables(links[1])) && Settings.Default.SuppressMissingDirectory == false)
                    {
                        MessageBox.Show("Directory\n" + ReplaceVariables(links[1]) + "\nfor link\n" + line + "\nDoes not exist.", ToolName);
                    } else if (!Directory.Exists(ReplaceVariables(links[1])))
                        Console.WriteLine("Directory\n" + ReplaceVariables(links[1]) + "\nfor link\n" + line + "\nDoes not exist.");
                }
            }

        }

        public static void SaveProfileLinks()
        {
            Console.WriteLine("\nExecuting Method: SaveProfileLinks");
            string ProfileLinks = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Links.txt";
            string SavedLinks = "Saving links for " + Settings.Default.ActiveProfile + "\n";
            foreach (string str in DirectoryLinks)
            {
                SavedLinks += str + "\n";
            }
            File.WriteAllLines(ProfileLinks, DirectoryLinks);

            Console.WriteLine(SavedLinks);
        }

        public static void LoadProfileExes()
        {
            Console.WriteLine("\nExecuting Method: LoadProfileExes");
            string ProfileExeDoc = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt";
            ProfileExes.Clear();
            if (File.Exists(ProfileExeDoc))
            {
                foreach (string s in File.ReadLines(ProfileExeDoc))
                {
                    string exe = s;
                    if (exe.IndexOf("|") == -1 && exe.IndexOf(";") != -1)
                        exe = exe.Replace(";", "|");
                    int cnt = exe.Length - exe.Replace("|","").Length;
                    Console.WriteLine("Found "+cnt+" of | in "+exe);

                    if (cnt == 0)
                        exe = Path.GetFileNameWithoutExtension(exe) + "|" + exe + "|";
                    else if (cnt == 1)
                        exe = Path.GetFileNameWithoutExtension(exe) + "|" + exe + "|" + exe.Split("|")[1];

                    ProfileExes.Add(exe);
                }
            }
            string CurrentExeList = Settings.Default.ActiveProfile + "'s EXEs: ";
            foreach (string exe in ProfileExes)
            {
                CurrentExeList += exe + ", ";
            }
            Console.WriteLine(CurrentExeList.Substring(0, CurrentExeList.Length - 2));

        }

        public static void SaveProfileExes()
        {
            Console.WriteLine("\nExecuting Method: SaveProfileExes");

            File.WriteAllLines(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt", ProfileExes);
        }

        public static void AmendExe(string exe)
        {
            Console.WriteLine("\nExecuting Method: AmendExe");
            ProfileExes.Add(Path.GetFileNameWithoutExtension(exe) + "|" + exe + "|");
            SaveProfileExes();
            Console.WriteLine("Ammended EXE " + SelectedExe + " to file " + Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt");
        }

        public static void LaunchExe()
        {
            Console.WriteLine("\nExecuting Method: LaunchExe");
            Console.WriteLine("Attempting to launch " + SelectedExe);
            if (Process.GetProcessesByName("VFSLauncher").Length == 0)
            {
                bool argspresent = false;
                string[] exe = ReplaceVariables(SelectedExe).Split("|");
                    if (exe[2] != "")
                    {
                        argspresent = true;
                    }
                if (!File.Exists(exe[1]))
                {
                    MessageBox.Show("Could not locate executable.\n" + exe[0], ToolName);
                    return;
                }

                if (File.Exists(envEXELOC + "\\usvfs\\VFSinstructions.txt"))
                    File.Delete(envEXELOC + "\\usvfs\\VFSinstructions.txt");

                using (StreamWriter instruct = new StreamWriter(envEXELOC + "\\usvfs\\VFSinstructions.txt"))
                {

                    foreach (string link in DirectoryLinks)
                    {
                        string ReplacedLink = ReplaceVariables(link.Replace("|",";"));
                        string[] linktolink = ReplacedLink.Split(";");

                        if (Directory.Exists(linktolink[0]) == false)
                        {
                            MessageBox.Show(linktolink[0] + "\ninvolved in link" + link + "\ndoes not exist.\n\nLaunch cancelled.", "Aborting Launch");
                            return;
                        }
                        if (Directory.Exists(linktolink[1]) == false)
                        {
                            MessageBox.Show(linktolink[1] + "\ninvolved in link" + link + "\ndoes not exist.\n\nLaunch cancelled.", "Aborting Launch");
                            return;
                        }

                        if (link == DirectoryLinks[DirectoryLinks.Count - 1])
                        {
                            instruct.Write(ReplacedLink);
                        }
                        else
                        {
                            instruct.Write(ReplacedLink + ";");
                        }
                    }
                    instruct.Close();
                }

                string launchargs = "\"" + envEXELOC + "\\usvfs\\VFSinstructions.txt\" \"" + exe[1] + "\" " + DateTime.Now.ToString("yyyyMMddHHmmssffff");

                Process VFS = new Process();
                VFS.StartInfo.FileName = envEXELOC + "\\usvfs\\VFSLauncher.exe";

                if (argspresent)
                {
                    Console.WriteLine("Launching " + exe[1] + " withs args " + exe[2]);
                    File.WriteAllText(envEXELOC + "\\usvfs\\launchargs.txt", exe[2]);
                    launchargs += " \"" + envEXELOC + "\\usvfs\\launchargs.txt" + "\"";
                }
                else
                {
                    launchargs += " noargs";
                    Console.WriteLine("Launching " + exe[1]);
                }

                if (CommonVars.Debug)
                    launchargs += " debug ";

                VFS.StartInfo.Arguments = launchargs;
                Console.WriteLine("VFSLauncher path " + VFS.StartInfo.FileName + " Command args " + VFS.StartInfo.Arguments);
                VFS.Start();
            }
            else
            {
                MessageBox.Show("Another program is already using our virtual file system.");
            }
        }

    }
}
