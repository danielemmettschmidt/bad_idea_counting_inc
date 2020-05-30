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

        public void SetRange(long Max, long Min, byte RangeDenominator)
        {            
            long Range = Max - Min;

            long Spot = Close - Min;

            int Percent = (int)((Spot * 1000) / Range);

            this.Fifty_Two_Week_Range = FindRange(Percent, RangeDenominator);
        }

        private byte FindRange(int Percent, byte RangeDenominator)
        {
            int Step = 1000 / RangeDenominator, Check = 0;

            byte ii = 1;

            while (Check < 1000)
            {
                Check = Step * ii;

                if (Check > Percent)
                {
                    return ii;
                }

                ii++;
            }

            return RangeDenominator;
        }

        ~SPY_Trading_Day()
        {

        }
    }
}
