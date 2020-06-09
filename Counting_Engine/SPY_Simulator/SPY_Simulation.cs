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
using System.Runtime.CompilerServices;

namespace SPY_Simulator
{
    public class SPY_Simulation
    {
        private bool run;
        
        public bool done;

        private SPY_History SPY;

        private Counting_Engine CE;

        private int startRange, incrementer, numberofvalues, numberofvariables, numberoftasks;

        private long dailyBuy, dailyBuyPrecision, sale, potCap, countCap, progress;
        
        public List<SPY_Simulation_Result> Results;

        public SPY_Simulation()
        {

        }

        public static SPY_Simulation DeserializeFromJSONFile(string file)
        {
            string jsonString = File.ReadAllText(file);

            JsonTextReader reader = new JsonTextReader(new StringReader(jsonString));

            JsonSerializer wtf = new JsonSerializer();

            return wtf.Deserialize<SPY_Simulation>(reader);

        }

        public SPY_Simulation(int NumberOfValues, int NumberOfVariables, int NumberOfTasks, int StartRangeAt, long DailyBuy, SPY_History SPYHistory)
        {
            // SET SIMULATION PARAMETERS 

            this.CE = new Counting_Engine(NumberOfValues, NumberOfVariables);

            SPYHistory.Filter_Data(SPYHistory.Trading_Days.Count - YearsToTradingDays(43));  // SIMULATION CURRENTLY TRIMS THE DATA DOWN THE LAST 43 YEARS 
                                                                                             // -> SOMEONE WHO STARTED INVESTING AT 22 AND IS RETIRING AT 65

            this.SPY = SPYHistory;

            this.numberoftasks = NumberOfTasks;

            this.numberofvalues = NumberOfValues;

            this.numberofvariables = NumberOfVariables;

            this.startRange = StartRangeAt;

            this.CalculateIncrementer();

            this.dailyBuy = DailyBuy * 100;

            this.dailyBuyPrecision = this.dailyBuy * 10000;

            this.sale = this.GrabSale();

            this.potCap = (this.dailyBuy * (YearsToTradingDays(1) / 4));  // POT CAP IS SET TO ONE FISCAL QUARTER'S WORTH OF BUYS

            this.countCap = Counting_Engine.ToPower(this.numberofvalues, this.numberofvariables);

            this.Results = new List<SPY_Simulation_Result>();

            this.progress = 0;
        }

        public void Run()
        {
            this.run = true;

            this.done = false;

            Thread sim = new Thread(this.RunTasks);

            sim.Start();
        }

        public void Stop()
        {
            this.run = false;
        }

        public void RunTasks()
        {
            List<Task<SPY_Simulation_Result>> Ts = new List<Task<SPY_Simulation_Result>>();

            // BEGIN RUNNING SIMULATION TASKS

            for (int ii = 0; ii < this.numberoftasks; ii++)
            {
                Ts.Add
                    (
                        Task<SPY_Simulation_Result>.Run(() =>
                        {
                            var threadId = Thread.CurrentThread.ManagedThreadId;
                            int taskId = ii;

                            return Run_Simulation(ii + 1);
                        })
                    );

                Thread.Sleep(1000);
            }

            for (int ii = 0; ii < this.numberoftasks; ii++)
            {
                Ts[ii].Wait();
                this.Results.Add(Ts[ii].Result);
            }

            if (this.Results.Count != 0)
            {
                this.SortResults();
            }

            this.done = true;
        }

        private void SortResults()
        {
            List<SPY_Simulation_Result> sortingResults = this.Results;

            int count = this.Results.Count;

            this.Results = new List<SPY_Simulation_Result>();

            long max = -1;
            
            int ii;

            while (this.Results.Count < this.numberoftasks)
            {
                this.Results.Add(GrabMaxResult(ref sortingResults, ref max, out ii));

                sortingResults.RemoveAt(ii);
            }
        }

        private SPY_Simulation_Result GrabMaxResult(ref List<SPY_Simulation_Result> SortingResults, ref long max, out int tii)
        {
            tii = 0;
            
            int ii = 0;

            SPY_Simulation_Result ret = new SPY_Simulation_Result(-1);

            foreach (SPY_Simulation_Result result in SortingResults)
            {
                if (result.Profit > max)
                {
                    max = result.Profit;
                    tii = ii;
                    ret = result;
                }

                ii++;
            }

            return ret;
        }

        
        
        
        
        
        
        
        
        
        // EVENTS

        public delegate void ProgressReportedEventHandler(SimulationReport report, EventArgs args);

        public event ProgressReportedEventHandler ProgressReported;
        protected virtual void ProgressReport(SimulationReport SR)
        {
            ProgressReported(SR, EventArgs.Empty);
        }










        // STRING REPORTING METHODS

        private string BuildCompletion(int input)
        {
            string build = ("" + input);

            while (build.Length < 5)
            {
                build = "0" + build;
            }

            return build;
        }

        private string ShowNumber(string Input, int Decimal)
        {
            char[] reverse = ReverseString(Input);

            int ii = 0;

            string build = "";

            foreach (char c in reverse)
            {
                if (ii == Decimal)
                {
                    build = build + ".";
                }

                if (ii > Decimal && (ii - Decimal) % 3 == 0)
                {
                    build = build + ",";
                }

                build = build + c;

                ii++;
            }

            reverse = ReverseString(build);

            build = "";

            foreach (char c in reverse)
            {
                build = build + c;
            }

            return build;
        }

        private string ShowNumber(long Input, int Decimal)
        {
            string read = ("" + Input);

            return ShowNumber(read, Decimal);
        }

        private char[] ReverseString(char[] input)
        {
            Array.Reverse(input);

            return input;
        }

        private char[] ReverseString(string input)
        {
            char[] reverse = input.ToCharArray();

            return ReverseString(reverse);

        }










        // SIMULATION METHODS

        private SPY_Simulation_Result Run_Simulation(int TaskNumber)
        {                              
            SPY_Simulation_Result result = new SPY_Simulation_Result(TaskNumber);

            long ii = progress + TaskNumber, tempCap = progress + TaskNumber;

            while (ii < this.countCap)
            {
                tempCap = tempCap + 1000;

                for (; ii < tempCap; ii = ii + this.numberoftasks)
                {
                    byte[] candidate = this.CE.buildRow(ii);

                    if (IsLogicalCandidate(candidate))
                    {
                        result = CalculateResult(result, candidate, ii, TaskNumber, tempCap);
                    }

                    if (this.progress < ii)
                    {
                        this.progress = ii;
                    }

                    if (this.run == false)
                    {
                        return result;
                    }
                }

                ProgressReport(new SimulationReport(ii, "Simulation has run " + ii + " times.", true));
            }

            

            return result;
        }

        private bool IsLogicalCandidate(byte[] Candidate)
        {
            int ignore = -1, ii = 0, halfway = ((Candidate.Length) / 2);

            int half = (this.numberofvalues / 2);

            bool foundGrower = false;

            if (Candidate.Length % 2 != 0)
            {
                ignore = halfway;
            }

            foreach (byte nominator in Candidate)
            {
                if (ii <= halfway)
                {
                    if (nominator > half)
                    {
                        foundGrower = true;
                    }
                }
                else
                {
                    if (ii != ignore)
                    {
                        if (nominator <= half && foundGrower == true)
                        {
                            return true;
                        }
                    }
                }

                ii++;
            }

            return false;
        }

        private SPY_Simulation_Result CalculateResult(SPY_Simulation_Result CurrentResult, byte[] Candidate, long ii, int TaskNumber, long CompletionDenom)
        {
            long profit = 0, surgePot = 0;

            foreach (SPY_Trading_Day day in this.SPY.Trading_Days)
            {
                byte nominator = (byte)(Candidate[(day.Fifty_Two_Week_Range - 1)]);

                profit = profit + CalculateProfit(day.Close, ref surgePot, nominator);
            }

            if (profit > CurrentResult.Profit)
            {
                CurrentResult.Profit = profit;
                CurrentResult.IteratorIndex = ii;
                CurrentResult.Result = Candidate;

                CurrentResult.Report = (DateTime.Now.ToString() + ": Task Number " + TaskNumber + " is " + ShowNumber( BuildCompletion( (int)(ii * 100000 / CompletionDenom) ) , 4) + "% complete, it has found a new max profit: $" + ShowNumber(profit, 2));



                Console.WriteLine(CurrentResult.Report);
            }

            return CurrentResult;
        }        

        private long CalculateProfit(long close, ref long SurgePot, byte Nominator)
        {
            int potFlow;

            Pot_Status_Is pot_status = CheckPot(SurgePot);

            potFlow = CalculatePotFlow(Nominator);

            long buy;

            if (    ( potFlow ==  1 && pot_status != Pot_Status_Is.Full  )
                 || ( potFlow == -1 && pot_status != Pot_Status_Is.Empty ) )
            {
                buy = CalculateSurgeBuy(Nominator);
                SurgePot = SurgePot + (this.dailyBuy - (buy / 100));
            }
            else
            {
                buy = this.dailyBuyPrecision;
            }

            int shares = (int)(buy / close);

            long proceeds = (shares * this.sale);

            return ((proceeds - buy) / 100);
        }

        private long CalculateSurgeBuy(byte nominator)
        {
            int percent = ( ( (nominator - 1) * this.incrementer ) + this.startRange );

            return ( ( this.dailyBuyPrecision * percent ) / 10000 );
        }

        private void CalculateIncrementer()
        {
            if (this.startRange < 0 || this.startRange >= 100)
            {
                throw new Exception(this.startRange + " was supplied for BeginRange argument for CalculateSurgeBuy... that number must be between 0 and 99");
            }

            int ranges = this.numberofvalues - 1;

            if ((this.startRange * 10) % ((ranges * 10) / 2) != 0)
            {
                throw new Exception(this.numberofvalues + " and " + this.startRange +
                                    " were supplied for NumberOfValues and for BeginRange arguments for CalculateSurgeBuy... " +
                                    "Check your math. (BeginRange) must be wholly divisible by ((NumberOfValues -1) / 2)");
            }

            this.incrementer = ((100 - this.startRange) / ranges) * 2;
        }

        private int CalculatePotFlow(byte nominator)
        {
            int intermediate = ( (nominator * 10) - ( (this.numberofvalues * 10 ) / 2) );

            if (intermediate < 5)
            {
                return (1);
            }

            if (intermediate < 10)
            {
                return 0;
            }

            return -1;
        }

        private Pot_Status_Is CheckPot(long pot)
        {
            if (pot <= 0)
            {
                return Pot_Status_Is.Empty;
            }

            if (pot >= this.potCap)
            {
                return Pot_Status_Is.Full;
            }

            return Pot_Status_Is.In_Flow;
        }

        private long GrabSale()
        {
            return (this.SPY.Trading_Days[(this.SPY.Trading_Days.Count - 1)].Close);
        }

        public static int YearsToTradingDays(int Years)
        {
            return (Years * 260);
        }
    }
}
