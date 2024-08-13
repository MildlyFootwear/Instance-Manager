using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Instance_Manager.Methods
{
    public class UpdateMethods
    {

        public static async void CheckGitVersion()
        {
            Console.WriteLine("\nExecuting Method: CheckGitVersion");
            string ver;
            Console.WriteLine("Checking github for latest version.");
            try
            {
                using HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/MildlyFootwear/Instance-Manager/master/ver.txt");
                response.EnsureSuccessStatusCode();
                ver = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Found " + ver);
                LatestVer = ver;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckGitVersion Exception: " + ex.Message);
                LatestVer = "Unknown";
            } 

        }

        public static void CheckForUpdate()
        {

            void threadMethod()
            {

                CheckGitVersion();

                while (LatestVer == null)
                {
                    Thread.Sleep(100);
                }

                if (LatestVer != "Unknown")
                {

                    if (LatestVer != Settings.Default.Version && LatestVer != Settings.Default.IngoreVersion)
                    {
                        var taskDialog = TaskDialog.ShowDialog(new TaskDialogPage
                        {
                            Caption = ToolName + " - "+Settings.Default.Version,
                            Text = LatestVer+" is available. Go to download page?",
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
                                Settings.Default.IngoreVersion = LatestVer;
                                Settings.Default.Save();
                                Console.WriteLine("Ignoring version " + LatestVer);
                            }
                        }
                    }
                    if (LatestVer == Settings.Default.IngoreVersion) { Console.WriteLine(LatestVer + " is ignored"); }
                    else if (LatestVer == Settings.Default.Version)
                    {
                        Console.WriteLine(Settings.Default.Version + " is up to date with repository version " + LatestVer);

                    }
                }
            }

            Thread C4U = new Thread(new ThreadStart(threadMethod));
            C4U.Start();

        }

    }
}
