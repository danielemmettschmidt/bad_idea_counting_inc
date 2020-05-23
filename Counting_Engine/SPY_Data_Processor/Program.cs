using System;
using System.Reflection;
using System.Security.Permissions;
using System.IO;
using System.Collections.Generic;

namespace SPY_Data_Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Assembly.GetAssembly(typeof(Program)).CodeBase;

            if (DirectoryValidator.DirectoryValidator.ProcessDirectoryArguments(args, ref dir) == DirectoryValidator.DirectoryValidationStatus.ValidDirectorySupplied)
            {
                dir = args[0];
            }

            if (DirectoryValidator.DirectoryValidator.ProcessDirectoryArguments(args, ref dir) != DirectoryValidator.DirectoryValidationStatus.Failed)
            {
                Process(dir);
            }
        }

        static void Process(string dir)
        {
            short ii = 0;

            byte CloseColNum = 0;

            List<string> FileRows = new List<string>();

            foreach (string line in File.ReadLines(DirectoryValidator.DirectoryValidator.getfilename(dir, ".csv")))
            {
                if (ii == 0)
                {
                    CloseColNum = grabIndex(line, "Close");
                }
                else
                {
                    FileRows.Add(line);
                }

                ii++;
            }

            ///////////////////////////////////HERE
        }

        static byte grabIndex(string titleline, string search)
        {
            byte ii = 0;

            foreach (string cell in titleline.ToLower().Split(','))
            {
                if (cell.Contains(search))
                {
                    return ii;
                }

                ii++;
            }

            throw new Exception("grabIndex function failed to find column name " + search);
        }

    }
}
