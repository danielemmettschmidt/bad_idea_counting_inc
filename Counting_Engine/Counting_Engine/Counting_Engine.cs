using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;

namespace Bad_Idea_Counting
{

    public class Counting_Engine
    {
        private int engineNumberOfValues;

        private int engineNumberOfVariables;

        private long engineDepth; 

        List<byte[]> chartRows;

        public Counting_Engine(int NumberOfValues, int NumberOfVariables)
        {
            initialize(NumberOfValues, NumberOfVariables, false, false);
        }

        public Counting_Engine(int NumberOfValues, int NumberOfVariables, bool construct)
        {
            initialize(NumberOfValues, NumberOfVariables, construct, false);
        }

        public Counting_Engine(int NumberOfValues, int NumberOfVariables, bool construct, bool display)
        {
            initialize(NumberOfValues, NumberOfVariables, construct, display);
        }

        private void initialize(int NumberOfValues, int NumberOfVariables, bool construct, bool display)
        {
            this.engineNumberOfValues = NumberOfValues;

            this.engineNumberOfVariables = NumberOfVariables;

            this.engineDepth = ToPower(this.engineNumberOfValues, this.engineNumberOfVariables);

            this.chartRows = new List<byte[]>();

            if (construct == true)
            {
                for (int ii = 1; ii <= this.engineDepth; ii++)
                {
                    byte[] bs = buildRow(ii);

                    chartRows.Add(bs);

                    if (display == true)
                    {
                        string displayString = (ii + OpeningSpaces(ii, 15));

                        foreach (byte b in bs)
                        {
                            displayString = displayString + MiddleSpaces(b);
                        }

                        Console.WriteLine(displayString);
                    }
                }
            }
        }

        private string OpeningSpaces(int ii, short sii)
        {
            string ret = "";
            
            for(; sii > 0; sii--)
            {
                ii = ii / 10;

                if(ii == 0)
                {
                    ret = ret + "-";
                }
            }

            return (" " + ret + ">");
        }

        private string MiddleSpaces(byte write)
        {
            if(write < 10)
            {
                return " | " + write + "| ";
            }
            else
            {
                return " |" + write + "| ";
            }
        }

        public byte[] buildRow(long y)
        {
            byte[] row = new byte[this.engineNumberOfVariables];

            int endianII = 0, placeII = this.engineNumberOfVariables;

            while (placeII > 0)
            {
                row[placeII - 1] = determineWriteValue(endianII, y, this.engineNumberOfValues, this.engineNumberOfVariables);

                endianII++;
                placeII--;
            }

            return row;
        }

        private byte counting(long y, int v)
        {
            if (y < v)
            {
                return (byte)y;
            }

            long A = LastModuloSatisfier(y, v);

            if (A == y)
            {
                return (byte)v;
            }

            return (byte)(y - A);
        }

        private byte determineWriteValue(in int x, in long y, in int v, in int n)
        {
            if (x == 0)
            {
                return counting(y, v);

            }
            else if (x > 0)
            {
                long A = ToPower(v, x);

                if (y <= A)
                {
                    return 1;
                }

                long b = LastModuloSatisfier((y - 1), A);

                long c = (b / A);  //////// c IS EFFECTIVELY THE 0 POWER Y 

                return counting(c + 1, v);
            }
            else
            {
                throw new Exception("x value was less than 0.");
            }
        }

        private long LastModuloSatisfier(long y, long search)
        {
            long ii = y;

            while (ii > 0)
            {
                if (ii % search == 0)
                {
                    return ii;
                }

                ii--;
            }

            throw new Exception("regCountingLastModuloSatisfier with y of " + y + " and search of " + search + " went below 0.");

        }

        public static long ToPower(int num, int power)
        {
            if (power == 0)
            {
                return 1;
            }

            long ret = num;

            while (power > 1)
            {
                ret = ret * num;

                power--;
            }

            return ret;
        }



    }

}
