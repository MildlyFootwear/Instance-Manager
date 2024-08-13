using Instance_Manager.UtilityForms;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Instance_Manager
{
    public class QuickLaunchM
    {
        public static void QuickLaunchMethod(ImmutableList<string> argsL)
        {

            int index = 0;

            Console.WriteLine("\nQuick launching\n");

            string passedexe = null;
            string passedprofile = null;

            foreach (string arg in argsL)
            {
                Console.WriteLine(arg);
                if (Path.GetExtension(arg) == ".exe")
                {
                    if (File.Exists(arg))
                    {
                        passedexe = arg;
                        Console.WriteLine("Found exe " + arg + " in args.");
                    }

                }
                else if (Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + arg))
                {
                    passedprofile = arg;
                    Console.WriteLine("Using profile " + arg);
                }
            }

            if (passedprofile == null)
            {
                Console.WriteLine("\nShowing quick profile");
                Form QuickProf = new QuickProfile();
                QuickProf.ShowDialog();
            }

            if (QuickLaunch)
                LoadProfileLinks();

            if (QuickLaunch)
            {
                if (passedexe == null)
                {
                    LoadProfileExes();
                    if (ProfileExes.Count > 0)
                    {
                        Form QuickExe = new QuickExe();
                        QuickExe.ShowDialog();
                    }
                    else { MessageBox.Show("No executables found for " + Settings.Default.ActiveProfile); return; }
                }
                else
                {
                    index = argsL.IndexOf(passedexe) + 1;
                    SelectedExe = FormattedExeFromPath(passedexe);
                    while (index < argsL.Count)
                    {
                        SelectedExe += " " + argsL[index];
                        index++;
                    }
                }
            }

            if (QuickLaunch)
                LaunchExe();

        }
    }
}
