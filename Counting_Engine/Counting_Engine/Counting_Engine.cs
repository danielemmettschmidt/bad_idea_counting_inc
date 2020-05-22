using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;

namespace Bad_Idea_Counting
{

    class Counting_Engine
    {
        public byte[,] values;

        private int surgePot;

        List<byte[]> chartRows;

        public Counting_Engine()
        {
            this.chartRows = new List<byte[]>();

            int input = 9;

            for (int ii = 1; ii <= ToPower(input, input); ii++)
            {
                byte[] bs = buildRow(ii, input, input);

                chartRows.Add(bs);

                string display = (ii + "");

                foreach (byte b in bs)
                {
                    display = display + "  " + b + "  ";
                }

                Console.WriteLine(display);
            }

        }

        public byte[] buildRow(long y, int numberOfValues, int numberOfVariables)
        {
            byte[] row = new byte[numberOfVariables];

            int endianII = 0, placeII = numberOfVariables;

            while (placeII > 0)
            {
                row[placeII - 1] = determineWriteValue(endianII, y, numberOfValues, numberOfVariables);

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

        private byte determineWriteValue(int x, long y, int v, int n)
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

        private static long ToPower(int num, int power)
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
