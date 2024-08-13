using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;


namespace usvfsWrap
{
    public class usvfsWrapM
    {

        static public uint LINKFLAG_FAILIFEXISTS = 0x00000001; // Linking fails in case of an error.
        static public uint LINKFLAG_MONITORCHANGES = 0x00000002; // Changes to the source directory after the link operation will be updated in the virtual fs. only relevant in static link directory operations.
        static public uint LINKFLAG_CREATETARGET = 0x00000004; // File creation (including move or copy) operations to destination will be redirected to the source. Only one createtarget can be set for a destination folder.
        static public uint LINKFLAG_RECURSIVE = 0x00000008; // Directories are linked recursively
        static public uint LINKFLAG_FAILIFSKIPPED = 0x00000010; // Linking fails if the file or directory is skipped files or directories are skipped depending on whats been added to the skip file suffixes or skip directories list.

        public enum LogLevel : uint
        {
            Debug, Info, Warning, Error
        };

        public enum CrashDumpsType : uint
        {
            None, Mini, Data, Full
        };

        /// <summary>
        /// Can be used to set debug mode for usvfsWrap functions, where it will print the name of functions as they execute along with the arguments passed to them.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern void usvfsWrapSetDebug(bool b);

        /// <summary>
        /// Safe method to initialize a VFS. Can only initialize one at a time, which will need to be freed with usvfsWrapFree once no longer in use.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern bool usvfsWrapCreateVFS(string instanceName, bool Debug, LogLevel logLevel, CrashDumpsType crashtype, string dumpPath, int delay);
        /// <summary>
        /// Frees the last created VFS.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern void usvfsWrapFree();

        /// <summary>
        /// exePath should be the full path to the executable. commandArgs will be the arguments passed to it as if they were right after it in a shortcut. See the documentation for CreateProcess for flag information.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern bool usvfsWrapCreateProcessHooked(string exePath, string commandArgs, byte createFlags, string workingDir);

        /// <summary>
        /// Returns the process ID of the last process launched by usvfsWrapCreateProcessHooked.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern int usvfsWrapGetLastHookedID();

        /// <summary>
        /// Links two directories in the VFS; source is the original/"real" directory, destination is where the files and potentially subdirectories will show for hooked applications. See LINKFLAG variables for flag information.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern void usvfsWrapVirtualLinkDirectoryStatic(string source, string destination, uint flags);

        /// <summary>
        /// Links two files in the VFS; source is the original/"real" file, destination is where the file will show up for hooked applications. See LINKFLAG variables for flag information.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern void usvfsWrapVirtualLinkFile(string source, string destination, uint flags);

        /// <summary>
        /// Creates a dump of the VFS to the text file specified by path. If the specified file is not found, it will create a default file in the working directory.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern bool usvfsWrapCreateVFSDump(string path);

        /// <summary>
        /// Returns an int representing the number of processes hooked into the VFS.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern int usvfsWrapGetHookedCount();

        /// <summary>
        /// Adds a suffix for files to skip. .txt and some_file.txt would both be considered valid.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern void usvfsWrapAddSkipFileSuffix(string suffix);

        /// <summary>
        /// Adds a string that will be skipped during directory linking. Not a path. All directories matching the name will be skipped. For example, if .git is added, any sub-path or root-path containing a.git directory will have the.git directory skipped during directory linking.
        /// </summary>
        [DllImport("usvfs\\usvfsWrap.dll")] public static extern void usvfsWrapAddSkipDirectory(string source, string destination);

    }
}
