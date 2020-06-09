using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPY_Data_Processor
{
    public class SPY_History
    {
        public static void SerializeClassToFile<T>(string Directory, T ob)
        {
            File.WriteAllText((Directory + "\\" + DateTime.UtcNow.ToString("MM_dd_yyyy_HH_mm_ss") + "_SPY_sim.progress"), JsonConvert.SerializeObject(ob));
        }

        public List<SPY_Trading_Day> Trading_Days;

        public SPY_History()
        {

        }

        public SPY_History(List<SPY_Trading_Day> Trading_Days_Input)
        {
            this.Trading_Days = Trading_Days_Input;
        }

        public SPY_History(List<string> FileRows, byte CloseColumnNumber, byte RangeDenominator)
        {
            this.Trading_Days = new List<SPY_Trading_Day>();

            foreach (string row in FileRows)
            {
                try
                {
                    long write = Int32.Parse(GrabCloseNoDecimal(row.Split(',')[CloseColumnNumber]));

                    this.Trading_Days.Add(new SPY_Trading_Day(write));
                }
                catch (Exception ex)
                {
                    throw new Exception("On writing '" + row + "' received exception -> " + ex.Message);
                }
            }

            long Current_Max = 0, Current_Min = 0;

            int Days_Left_On_Max = 260, Days_Left_On_Min = 0, ii = 0;

            List<long> Fifty_Two_Week_Closes = new List<long>();

            foreach (SPY_Trading_Day Day in this.Trading_Days)
            {
                if (ii < 260)
                {
                    if (ii == 0)
                    {
                        Current_Max = Day.Close + 1;
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

                this.Trading_Days[ii].SetRange(Current_Max, Current_Min, RangeDenominator);

                Days_Left_On_Max--;
                Days_Left_On_Min--;
                ii++;
            }
        }

        private string GrabCloseNoDecimal(string input)
        {
            string ret = "";

            int ii = 0;

            bool foundDecimal = false;

            foreach(char c in input)
            {
                if(("" + c) == ".")
                {
                    foundDecimal = true;
                }
                else
                {
                    ret = ret + c;
                }

                if (ii == 2)
                {
                    return ret;
                }

                if(foundDecimal == true)
                {
                    ii++;
                }
            }

            return ret;
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

        public static SPY_History DeserializeFromJSONFile(string file)
        {
            string jsonString = File.ReadAllText(file);

            JsonTextReader reader = new JsonTextReader(new StringReader(jsonString));

            JsonSerializer wtf = new JsonSerializer();

            return wtf.Deserialize<SPY_History>(reader);

        }

        public void Filter_Data(int StartAt)
        {
            this.Filter_Data(StartAt, (this.Trading_Days.Count - 1)); 
        }

        public void Filter_Data(int StartAt, int EndAt)
        {
            List<SPY_Trading_Day> Days = new List<SPY_Trading_Day>();

            int ii = StartAt;

            while(ii < this.Trading_Days.Count)
            {
                if(ii <= EndAt)
                {
                    Days.Add(this.Trading_Days[ii]);
                }

                ii++;
            }

            this.Trading_Days = Days;
        }

        ~SPY_History()
        {

        }
    }
}
