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
            int index = 0;
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

        public static string CheckGitVersion()
        {
            string ver;
            WebClient client = new();
            Console.WriteLine("Checking github for latest version.");
            try
            {
                Stream stream = client.OpenRead("https://raw.githubusercontent.com/MildlyFootwear/Instance-Manager/master/ver.txt");
                StreamReader reader = new StreamReader(stream);
                ver = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                Console.WriteLine("Found " + ver);
                return ver;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: "+ex.Message);
                return null;
            }

        }
        public static string InsertVariables(string path)
        {
            Console.WriteLine("\nInserting variables");

            int index = 0;
            foreach (string s in SystemVariablesValues)
            {
                Console.WriteLine("Checking " + path + " for " + s + " to replace with" + SystemVariables[index]);
                path = path.Replace(s, SystemVariables[index]);
                index++;
            }
            Console.WriteLine();
            return path;

        }

        public static string ReplaceVariables(string path)
        {
            Console.WriteLine("\nReplacing variables for " + path);

            int index = 0;

            foreach (string s in SystemVariables)
            {
                Console.WriteLine("Checking for " + s + " to replace with " + SystemVariablesValues[index]);
                path = path.Replace(s, SystemVariablesValues[index]);
                index++;
            }

            Console.WriteLine();
            return path;
        }

        public static void LoadProfiles()
        {

            Console.WriteLine("\nLoading profiles");

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
            Console.WriteLine(CurrentProfileList.Substring(0, CurrentProfileList.Length - 2) + ".\n");

        }

        public static void SetProfile(string prof)
        {
            Console.WriteLine();

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
            Console.WriteLine();
            profPATH = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile;
            SystemVariablesValues[SystemVariables.IndexOf("%ACTIVEPROFILE%")] = profPATH;
            string ProfileLinks = profPATH + "\\Links.txt";
            Console.WriteLine("\nLoading links for " + Settings.Default.ActiveProfile + "\n");
            DirectoryLinks.Clear();
            if (File.Exists(ProfileLinks))
            {
                var lines = File.ReadLines(ProfileLinks);
                foreach (var line in lines)
                {
                    DirectoryLinks.Add(line);
                    string[] links = line.Split(";");
                    Console.WriteLine(line.Replace(";", " to "));
                    if (!Directory.Exists(ReplaceVariables(links[0])))
                        MessageBox.Show("Directory\n" + ReplaceVariables(links[0]) + "\nfor link\n" + line + "\nDoes not exist.");

                    if (!Directory.Exists(ReplaceVariables(links[1])))
                        MessageBox.Show("Directory\n" + ReplaceVariables(links[1]) + "\nfor link\n" + line + "\nDoes not exist.");
                }
            }

        }

        public static void SaveProfileLinks()
        {
            Console.WriteLine();
            string ProfileLinks = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Links.txt";
            string SavedLinks = "Saving links for " + Settings.Default.ActiveProfile + "\n";
            foreach (string str in DirectoryLinks)
            {
                SavedLinks += str + "\n";
            }
            File.WriteAllLines(ProfileLinks, DirectoryLinks);

            Console.WriteLine(SavedLinks + "\n");
        }

        public static void LoadProfileExes()
        {
            Console.WriteLine();
            string ProfileExeDoc = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt";
            ProfileExes.Clear();
            if (File.Exists(ProfileExeDoc))
            {
                foreach (string exe in File.ReadLines(ProfileExeDoc))
                {
                    ProfileExes.Add(exe);
                }
            }
            string CurrentExeList = Settings.Default.ActiveProfile + "'s EXEs: ";
            foreach (string exe in ProfileExes)
            {
                CurrentExeList += exe + ", ";
            }
            Console.WriteLine(CurrentExeList.Substring(0, CurrentExeList.Length - 2) + ".\n");

        }

        public static void SaveProfileExes()
        {
            Console.WriteLine("\nSaving profile exes\n");

            File.WriteAllLines(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt", ProfileExes);
        }

        public static void AmendExe(string amend)
        {
            ProfileExes.Add(amend);
            SaveProfileExes();
            Console.WriteLine("\nAmmended EXE " + SelectedExe + " to file " + Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt\n");
        }

        public static void LaunchExe()
        {

            Console.WriteLine("\nAttempting to launch " + SelectedExe);


            if (Process.GetProcessesByName("VFSLauncher").Length == 0)
            {
                bool argspresent = false;
                bool colonpresent = false;
                if (SelectedExe.IndexOf(";") != -1)
                {
                    colonpresent = true;
                }
                string[] exe = ReplaceVariables(SelectedExe).Split(";");
                if (colonpresent)
                {
                    if (exe[1] != "")
                    {
                        argspresent = true;
                    }
                }
                if (!File.Exists(exe[0]))
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
                        string ReplacedLink = ReplaceVariables(link);
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

                string launchargs = "\"" + envEXELOC + "\\usvfs\\VFSinstructions.txt\" \"" + exe[0] + "\" " + DateTime.Now.ToString("yyyyMMddHHmmssffff");

                Process VFS = new Process();
                VFS.StartInfo.FileName = envEXELOC + "\\usvfs\\VFSLauncher.exe";

                if (argspresent)
                {
                    Console.WriteLine("Launching " + exe[0] + " withs args " + exe[1]);
                    File.WriteAllText(envEXELOC + "\\usvfs\\launchargs.txt", exe[1]);
                    launchargs += " \"" + envEXELOC + "\\usvfs\\launchargs.txt" + "\"";
                }
                else
                {
                    launchargs += " noargs";
                    Console.WriteLine("Launching " + exe[0]);
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
