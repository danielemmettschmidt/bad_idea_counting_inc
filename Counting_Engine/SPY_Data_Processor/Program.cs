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
            ///////// ADD DENOMINATOR PROCESSOR
            ///
            args = new string[1];
            args[0] = "C:\\Dev\\spydata";  //// yuck yuck yuck

            string dir = Assembly.GetAssembly(typeof(Program)).CodeBase;

            if (DirectoryValidator.DirectoryValidator.ProcessDirectoryArguments(args, ref dir) == DirectoryValidator.DirectoryValidationStatus.ValidDirectorySupplied)
            {
                dir = args[0];
            }

            if (DirectoryValidator.DirectoryValidator.ProcessDirectoryArguments(args, ref dir) != DirectoryValidator.DirectoryValidationStatus.Failed)
            {
                Process(dir, 8);  ///////////// LET'S NOT HARDCODE THIS
            }
        }

        static void Process(string dir, byte RangeDenominator)
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

            SPY_History SPY = new SPY_History(FileRows, CloseColNum, RangeDenominator);
        }

        static byte grabIndex(string titleline, string search)
        {
            byte ii = 0;

            foreach (string cell in titleline.Split(','))
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
