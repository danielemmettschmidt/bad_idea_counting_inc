using System;
using System.Collections.Generic;
using System.Text;

namespace SPY_Data_Processor
{
    public class SPY_Trading_Day
    {
        public long Close;
        public byte Fifty_Two_Week_Range;

        public SPY_Trading_Day(long TradingDayClose)
        {
            this.Close = TradingDayClose;
        }

        public void SetRange(long Max, long Mine)
        {


            this.Fifty_Two_Week_Range = Range; 
        }

        ~SPY_Trading_Day()
        {

        }
    }
}
