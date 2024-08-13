using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Instance_Manager.Methods
{
    internal class LinkModifierMethods
    {

        public static void EditLink(int index, bool second)
        {
            Console.WriteLine("\nExecuting Method: EditLink");
            Console.Write("Index is "+index);
            if (second) Console.WriteLine(". Editing destination."); else Console.WriteLine(". Editing source.");
            string link = ProfileDirectoryLinks[index];
            string[] splitlink = link.Split("|");
            string linksection;
            Form TextIn = new DirectoryTextEditor();
            if (second)
                linksection = splitlink[1];
            else
                linksection = splitlink[0];
            TextIn.Text = linksection;
            TextIn.ShowDialog();
            if (TextInputString == "Remove")
            {
                ProfileDirectoryLinks.Remove(link);
                SaveProfileLinks();
                NeedLinkMenuRefresh = true;
            }
            else if (TextInputString != linksection && TextInputString != "Cancel" && TextInputString != "")
            {
                if (Directory.Exists(ReplaceVariables(TextInputString)))
                {
                    if (second)
                        ProfileDirectoryLinks[ProfileDirectoryLinks.IndexOf(link)] = splitlink[0] + "|" + TextInputString;
                    else
                        ProfileDirectoryLinks[ProfileDirectoryLinks.IndexOf(link)] = TextInputString + "|" + splitlink[1];

                    SaveProfileLinks();
                    NeedLinkMenuRefresh = true;

                }
                else
                {
                    if (MessageBox.Show("Could not find directory " + ReplaceVariables(TextInputString) + ". Create directory?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Directory.CreateDirectory(ReplaceVariables(TextInputString));
                        if (second)
                            ProfileDirectoryLinks[ProfileDirectoryLinks.IndexOf(link)] = splitlink[0] + "|" + TextInputString;
                        else
                            ProfileDirectoryLinks[ProfileDirectoryLinks.IndexOf(link)] = TextInputString + "|" + splitlink[1];
                        SaveProfileLinks();
                        NeedLinkMenuRefresh = true;

                    }
                }
            }
            else
            {
                Console.WriteLine("Canceling link edit for " + link);
            }
        }

    }
}
