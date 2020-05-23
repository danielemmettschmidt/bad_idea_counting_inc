using System;
using System.Collections.Generic;
using System.Text;

namespace SPY_Data_Processor
{
    public class SPY_History
    {
        public List<SPY_Trading_Day> Trading_Days;

        public SPY_History(List<string> FileRows, byte CloseColumnNumber, byte RangeDenominator)
        {
            this.Trading_Days = new List<SPY_Trading_Day>();

            foreach (string row in FileRows)
            {
                try
                {
                    int write = Int32.Parse(row.Split(',')[CloseColumnNumber]);

                    this.Trading_Days.Add(new SPY_Trading_Day(write));
                }
                catch (Exception ex)
                {
                    throw new Exception("On row '" + row + "' received exception -> " + ex.Message);
                }
            }

            long Current_Max = 0, Current_Min = 0;

            int Days_Left_On_Max = 0, Days_Left_On_Min = 0, ii = 0;

            List<long> Fifty_Two_Week_Closes = new List<long>();

            foreach (SPY_Trading_Day Day in this.Trading_Days)
            {
                if (ii < 260)
                {
                    if (ii == 0)
                    {
                        Current_Max = Day.Close;
                        Current_Min = Day.Close;
                    }

                    Fifty_Two_Week_Closes.Add(Day.Close);
                }
                else
                {
                    Fifty_Two_Week_Closes.RemoveAt(0);
                    Fifty_Two_Week_Closes.Add(Day.Close);
                }

                if(Day.Close >= Current_Max)
                {
                    Current_Max = Day.Close;
                    Days_Left_On_Max = 260;
                }

                if(Day.Close <= Current_Min)
                {
                    Current_Min = Day.Close;
                    Days_Left_On_Min = 260;
                }

                if (Days_Left_On_Max == 0)
                {
                    Current_Max = SearchForNewMax(Fifty_Two_Week_Closes, out Days_Left_On_Max);
                }

                if (Days_Left_On_Min == 0)
                {
                    Current_Min = SearchForNewMin(Fifty_Two_Week_Closes, out Days_Left_On_Min);
                }

                this.Trading_Days[ii].SetRange(Current_Max, Current_Min);

                Days_Left_On_Max--;
                Days_Left_On_Min--;
                ii++;
            }
        }

        private long SearchForNewMax(List<long> History, out int Days_Left_On_Max)
        {
            int ii = 0;

            Days_Left_On_Max = 0;

            long NewMax = 0;

            foreach(long Close in History)
            {
                if(Close >= NewMax)
                {
                    Days_Left_On_Max = 260 - ii;
                    NewMax = Close;
                }

                ii++;
            }

            return NewMax;
        }

        private long SearchForNewMin(List<long> History, out int Days_Left_On_Min)
        {
            int ii = 0;

            Days_Left_On_Min = 0;

            long NewMin = History[0];

            foreach (long Close in History)
            {
                if (Close <= NewMin)
                {
                    Days_Left_On_Min = 260 - ii;
                    NewMin = Close;
                }

                ii++;
            }

            return NewMin;
        }

        ~SPY_History()
        {

        }
    }
}
