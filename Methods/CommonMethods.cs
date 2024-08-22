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

        public static void WriteLineIfDebug(string s = "")
        {
            if (ToolDebug)
                Console.WriteLine(s);
        }

        public static void WriteIfDebug(string s = "")
        {
            if (ToolDebug)
                Console.Write(s);
        }

        public static void ThreadedMessage(string Message, string TitleAppend = "")
        {
            void msgbx()
            {
                MessageBox.Show(Message, ToolName+TitleAppend);
            }
            Thread msg = new Thread(msgbx);
            msg.Start();
        }

        public static void SetDriveVariables()
        {
            WriteLineIfDebug("\nExecuting Method: SetDriveVariables");
            DriveInfo[] Drives = DriveInfo.GetDrives();
            foreach (DriveInfo d in Drives)
            {
                if (!d.IsReady)
                {
                    MessageBox.Show("A drive was not ready to be read from. Recommend restarting the program. If the problem persists, check your drive to ensure it is not failing. If the drive is fine, report to \".shoey\" on Discord or MildlyFootwear on Nexus Mods.", ToolName);
                    continue;
                }
                if (File.Exists(d.Name+"\\Instance-Manager\\DriveLabel.txt"))
                {
                    string s = "%CustomLabel "+File.ReadAllLines(d.Name + "\\Instance-Manager\\DriveLabel.txt")[0]+"%";
                    WriteLineIfDebug("    Assigning label "+s+" for "+d.Name);
                    SystemVariables.Add(s);
                    SystemVariablesValues.Add(d.Name.Substring(0, 2));

                }
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
                    MessageBox.Show("Variable " + varLabel + " is not free to assign for drive " + d.Name+"\n"+ToolName+" will only be able to refer to it with "+d.Name, ToolName+" - Minor Error");
                }

            }
        }

        public static string InsertVariables(string path)
        {
            WriteLineIfDebug("\nExecuting Method: InsertVariables");
            WriteLineIfDebug("    Inserting variables for " + path);
            int index = 0;
            foreach (string s in SystemVariablesValues)
            {
                //WriteLineIfDebug("Checking " + path + " for " + s + " to replace with" + SystemVariables[index]);
                path = path.Replace(s, SystemVariables[index]);
                index++;
            }
            WriteLineIfDebug("    Returning " + path);
            return path;

        }

        public static string ReplaceVariables(string path)
        {
            WriteLineIfDebug("\nExecuting Method: ReplaceVariables");
            WriteLineIfDebug("    Replacing variables for " + path);

            int index = 0;

            foreach (string s in SystemVariables)
            {
                //WriteLineIfDebug("Checking for " + s + " to replace with " + SystemVariablesValues[index]);
                path = path.Replace(s, SystemVariablesValues[index]);
                index++;
            }

            WriteLineIfDebug("    Returning "+path);
            return path;
        }

        public static void LoadProfiles()
        {

            WriteLineIfDebug("\nExecuting Method: LoadProfiles");

            Profiles.Clear();
            if (!Directory.Exists(Settings.Default.ProfilesDirectory))
            {
                Directory.CreateDirectory(Settings.Default.ProfilesDirectory);
                WriteLineIfDebug("    Created profiles directory.");
            }
            if (!Directory.EnumerateDirectories(Settings.Default.ProfilesDirectory).Any())
            {
                Directory.CreateDirectory(Settings.Default.ProfilesDirectory + "\\Default");
                Settings.Default.ActiveProfile = "Default";
                Settings.Default.Save();
                MessageBox.Show("No profiles found. Created default profile.", ToolName);
                WriteLineIfDebug("    Created default profile.");
            }

            foreach (string prof in Directory.EnumerateDirectories(Settings.Default.ProfilesDirectory))
            {
                Profiles.Add(prof.Replace(Settings.Default.ProfilesDirectory + "\\", ""));
                WriteLineIfDebug("    Added " + prof.Replace(Settings.Default.ProfilesDirectory + "\\", "") + " to profile list.");
            }

            string CurrentProfileList = "Profile options: ";
            foreach (string prof in Profiles)
            {
                CurrentProfileList += prof + ", ";
            }
            WriteLineIfDebug(CurrentProfileList.Substring(0, CurrentProfileList.Length - 2));

        }

        public static void SetProfile(string prof)
        {
            WriteLineIfDebug("\nExecuting Method: SetProfile");
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
            WriteLineIfDebug("    Updated ActiveProfile to " + Settings.Default.ActiveProfile + ". Passed profile was " + prof + "\n");
            LoadProfileLinks();
            LoadProfileExes();

        }

        public static void LoadProfileLinks()
        {
            WriteLineIfDebug("\nExecuting Method: LoadProfileLinks");
            profPATH = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile;
            SystemVariablesValues[SystemVariables.IndexOf("%ACTIVEPROFILE%")] = profPATH;
            string ProfileLinks = profPATH + "\\Links.txt";
            WriteLineIfDebug("    Loading links for " + Settings.Default.ActiveProfile);
            ProfileDirectoryLinks.Clear();
            if (File.Exists(ProfileLinks))
            {
                var lines = File.ReadLines(ProfileLinks);
                foreach (var l in lines)
                {
                    string line = l;
                    if (line.IndexOf("|") == -1)
                    {
                        line = line.Replace(";", "|");
                        UpdatedFormat = true;
                    }
                    ProfileDirectoryLinks.Add(line);
                    string[] links = ReplaceVariables(line).Split("|");
                    WriteLineIfDebug("\n    "+line.Replace("|", " to "));
                    if (!Directory.Exists((links[0])) && Settings.Default.SuppressMissingDirectory == false)
                    {
                        MessageBox.Show("Directory\n" + (links[0]) + "\nfor link\n" + line + "\nDoes not exist.", ToolName);
                    } else if (!Directory.Exists((links[0])))
                        WriteLineIfDebug("Directory\n" + (links[0]) + "\nfor link\n" + line + "\nDoes not exist.");
                    if (!Directory.Exists((links[1])) && Settings.Default.SuppressMissingDirectory == false)
                    {
                        MessageBox.Show("Directory\n" + (links[1]) + "\nfor link\n" + line + "\nDoes not exist.", ToolName);
                    } else if (!Directory.Exists((links[1])))
                        WriteLineIfDebug("    Directory\n" + (links[1]) + "\nfor link\n" + line + "\nDoes not exist.");
                }
                if (UpdatedFormat)
                {
                    WriteLineIfDebug("    Updated formatting, rewriting...");
                    UpdatedFormat = false;
                    SaveProfileLinks();
                }
            }

        }

        public static void SaveProfileLinks()
        {
            WriteLineIfDebug("\nExecuting Method: SaveProfileLinks");
            string ProfileLinks = Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Links.txt";
            string SavedLinks = "    Saving links for " + Settings.Default.ActiveProfile + "\n";
            foreach (string str in ProfileDirectoryLinks)
            {
                SavedLinks += "    "+str + "\n";
            }
            File.WriteAllLines(ProfileLinks, ProfileDirectoryLinks);

            WriteLineIfDebug(SavedLinks);
        }

        public static string FormattedExeFromPath(string path)
        {
            string s = Path.GetFileNameWithoutExtension(path) + "|" + path + "|"+Path.GetDirectoryName(path)+"|";
            if (ToolDebug)
                WriteLineIfDebug("\nFormattedExeFromPath:\n    "+path+"\n    "+s);
            return s;
        }

        public static void LoadProfileExes()
        {
            WriteLineIfDebug("\nExecuting Method: LoadProfileExes");
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
                    WriteLineIfDebug("    Found " + cnt+" of | in "+exe);

                    if (cnt == 0)
                        { WriteIfDebug("    Updating format for\n    " + exe); exe = Path.GetFileNameWithoutExtension(exe) + "|" + exe + "|" + Path.GetDirectoryName(exe) + "|"; UpdatedFormat = true; WriteLineIfDebug("\nUpdated to\n    " + exe); }
                    else if (cnt == 1)
                        { WriteIfDebug("    Updating format for\n    " + exe); exe = Path.GetFileNameWithoutExtension(exe.Split("|")[0]) + "|" + exe.Split("|")[0] + "|" + Path.GetDirectoryName(exe) + "|" + exe.Split("|")[1]; UpdatedFormat = true; WriteLineIfDebug("\nUpdated to\n    " + exe); }
                    else if (cnt == 2)
                        { WriteIfDebug("    Updating format for\n    " + exe); exe = exe.Split("|")[0] + "|" + exe.Split("|")[1] + "|" + Path.GetDirectoryName(exe.Split("|")[1]) + "|" + exe.Split("|")[2]; UpdatedFormat = true; WriteLineIfDebug("\nUpdated to\n    " + exe); }
                    ProfileExes.Add(exe);
                }
            }
            string CurrentExeList = "    "+Settings.Default.ActiveProfile + "'s EXEs: ";
            foreach (string exe in ProfileExes)
            {
                CurrentExeList += exe + ", ";
            }
            WriteLineIfDebug(CurrentExeList.Substring(0, CurrentExeList.Length - 2));
            if (UpdatedFormat)
            {
                WriteLineIfDebug("    Updated formatting, rewriting...");
                UpdatedFormat = false;
                SaveProfileExes();
            }

        }

        public static void SaveProfileExes()
        {
            WriteLineIfDebug("\nExecuting Method: SaveProfileExes");

            File.WriteAllLines(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt", ProfileExes);
        }

        public static void AmendExe(string exe)
        {
            WriteLineIfDebug("\nExecuting Method: AmendExe");
            ProfileExes.Add(FormattedExeFromPath(exe));
            SaveProfileExes();
            WriteLineIfDebug("    Ammended EXE " + SelectedExe + " to file " + Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile + "\\Exes.txt");
        }


    }
}
