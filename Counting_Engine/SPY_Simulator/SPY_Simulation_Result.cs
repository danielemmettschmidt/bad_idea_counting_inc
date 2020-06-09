using System;
using System.Collections.Generic;
using System.Text;

namespace SPY_Simulator
{    public class SPY_Simulation_Result
    {
        public int TaskNumber;
        public long Profit;
        public long IteratorIndex;
        public byte[] Result;
        public string Report;

        public SPY_Simulation_Result(int TaskNumber)
        {
            this.TaskNumber = TaskNumber;
            this.Profit = 0;
            this.IteratorIndex = 0;
            this.Result = new byte[0];
        }

        ~SPY_Simulation_Result()
        {

        }
    }
}
