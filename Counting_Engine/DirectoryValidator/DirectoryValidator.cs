using System;
using System.Reflection;
using System.Security.Permissions;
using System.IO;
using System.Linq;

namespace DirectoryValidator
{

    public enum DirectoryValidationStatus
    {
        ValidDirectorySupplied,
        FallBackToAssemblyDirectory,
        Failed
    }

    public class DirectoryValidator
    {
        public static DirectoryValidationStatus ProcessDirectoryArguments(string[] args, ref string AssemblyDirectory)
        {
            string dir = "";

            bool hasArgs;

            if (args.Length == 0)
            {
                hasArgs = false;
            }
            else
            {
                hasArgs = true;
            }

            if (hasArgs == false)
            {
                dir = ImprovedGetAssembly( ref AssemblyDirectory);
            }
            else
            {
                dir = args[0];
            }

            if (valdir(dir) == false)
            {
                return DirectoryValidationStatus.Failed;
            }
            else
            {
                if (hasArgs == true)
                {
                    return DirectoryValidationStatus.ValidDirectorySupplied;
                }
                else
                {
                    return DirectoryValidationStatus.FallBackToAssemblyDirectory;
                }
            }
        }

        private static string ImprovedGetAssembly(ref string AssemblyDirectory)
        {
            string start = Path.GetDirectoryName(AssemblyDirectory);

            if (start.Contains("file:\\file:") == true)
            {
                start = start.Replace("file:\\file:", "");
            }

            if (start.Contains("file:\\") == true)
            {
                start = start.Replace("file:\\", "");
            }

            AssemblyDirectory = start;

            return start;
        }

        private static bool valdir(string input)
        {
            if (Directory.Exists(input))
            {
                return true;
            }
            else
            {
                Console.WriteLine("\nDirectory does not exist.\n");
                return false;
            }
        }

        public static string getfilename(string dir, string search)
        {
            foreach (string filename in Directory.GetFiles(dir))
            {
                if (filename.Contains(search))
                {
                    return filename;
                }
            }

            throw new Exception("No file with ending \"" + search + "\" found in " + dir + ".");
        }

        public static string getfiledir(string filepath)
        {
            string ret = "", build = "";
            foreach (char c in filepath)
            {
                if (("" + c) == "\\")
                {
                    ret = ret + build + "\\";
                }
                else
                {
                    build = build + c;
                }
            }

            return ret;
        }
    }
}
