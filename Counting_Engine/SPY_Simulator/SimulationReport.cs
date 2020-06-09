using Bad_Idea_Counting;
using SPY_Data_Processor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPY_Simulator
{
    enum Pot_Status_Is
    {
        Full,
        Empty,
        In_Flow
    }

    public class SimulationReport
    {
        public string ReportString;
        public long CurrentProgress;
        public bool IsSilent;

        public SimulationReport(long Progress, bool Silent)
        {
            this.ReportString = "";
            this.CurrentProgress = Progress;
            this.IsSilent = Silent;
        }

        public SimulationReport(long Progress, string ReportString, bool Silent)
        {
            this.ReportString = ReportString;
            this.CurrentProgress = Progress;
            this.IsSilent = Silent;

        }

        ~SimulationReport()
        {

        }
    }
}