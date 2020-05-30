using Bad_Idea_Counting;
using SPY_Data_Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPY_Simulator
{
    enum Pot_Status_Is
    {
        Full,
        Empty,
        In_Flow
    }

    class SPY_Simulation
    {
        private SPY_History SPY;

        private Counting_Engine CE;

        private int startRange, incrementer;

        private long dailyBuy, dailyBuyPrecision, sale, potCap;

        public List<SPY_Simulation_Result> Results;

        public SPY_Simulation(int NumberOfValues, int NumberOfVariables, int NumberOfTasks, int StartRangeAt, long DailyBuy, SPY_History SPYHistory)
        {
            // SET SIMULATION PARAMETERS 

            this.CE = new Counting_Engine(NumberOfValues, NumberOfVariables);

            SPYHistory.Filter_Data(SPYHistory.Trading_Days.Count - YearsToTradingDays(43));  // SIMULATION CURRENTLY TRIMS THE DATA DOWN THE LAST 43 YEARS 
                                                             // -> SOMEONE WHO STARTED INVESTING AT 22 AND IS RETIRING AT 65

            this.SPY = SPYHistory;

            this.startRange = StartRangeAt;

            this.CalculateIncrementer(NumberOfValues, StartRangeAt);

            this.dailyBuy = DailyBuy * 100;

            this.dailyBuyPrecision = this.dailyBuy * 10000;

            this.sale = this.GrabSale();

            this.potCap = (this.dailyBuy * (YearsToTradingDays(1) / 4));  // POT CAP IS SET TO ONE FISCAL QUARTER'S WORTH OF BUYS

            // BEGIN RUNNING SIMULATION TASKS

            List<Task<SPY_Simulation_Result>> Ts = new List<Task<SPY_Simulation_Result>>();

            this.Results = new List<SPY_Simulation_Result>();

            for (int ii = 0; ii < NumberOfTasks; ii++)
            {
                Ts.Add
                    (
                        Task<SPY_Simulation_Result>.Run( () =>
                        {
                            var threadId = Thread.CurrentThread.ManagedThreadId;
                            int taskId = ii;

                            return Run_Simulation(NumberOfValues, NumberOfVariables, (ii + 1), NumberOfTasks);
                        })
                    );

                Thread.Sleep(1000);
            }

            for (int ii = 0; ii < NumberOfTasks; ii++)
            {
                Ts[ii].Wait();
                this.Results.Add(Ts[ii].Result);
            }

            this.SortResults();            
        }

        private void SortResults()
        {
            List<SPY_Simulation_Result> sortingResults = this.Results, intermediateResults = new List<SPY_Simulation_Result>();

            int count = this.Results.Count;

            this.Results = new List<SPY_Simulation_Result>();

            while(this.Results.Count != count)
            {
                long max = 0;
                int target = 0;

                foreach (SPY_Simulation_Result result in sortingResults)
                {
                    if(result.Profit > max)
                    {
                        max = result.Profit;
                        target = result.TaskNumber;
                    }
                }

                foreach (SPY_Simulation_Result result in sortingResults)
                {
                    if(result.TaskNumber == target)
                    {
                        this.Results.Add(result);
                    }
                    else
                    {
                        intermediateResults.Add(result);
                    }
                }

                sortingResults = intermediateResults;
            }
        }



        private SPY_Simulation_Result Run_Simulation(int NumberOfValues, int NumberOfVariables, int TaskNumber, int NumberOfTasks)
        {
            long countCap = Counting_Engine.ToPower(NumberOfValues, NumberOfVariables);

            SPY_Simulation_Result result = new SPY_Simulation_Result(TaskNumber);

            for (long ii = TaskNumber; ii < countCap; ii = ii + NumberOfTasks)
            {
                byte[] candidate = this.CE.buildRow(ii);

                if(IsLogicalCandidate(candidate, NumberOfValues))
                {
                    result = CalculateResult(result, candidate, NumberOfValues, ii, TaskNumber, countCap);
                }                                
            }

            return result;
        }

        private bool IsLogicalCandidate(byte[] Candidate, int NumberOfValues)
        {
            int ignore = -1, ii = 0, halfway = ((Candidate.Length) / 2);

            int half = (NumberOfValues / 2);

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

        private SPY_Simulation_Result CalculateResult(SPY_Simulation_Result CurrentResult, byte[] Candidate, int NumberOfValues, long ii, int TaskNumber, long CompletionDenom)
        {
            long profit = 0, surgePot = 0;

            foreach (SPY_Trading_Day day in this.SPY.Trading_Days)
            {
                byte nominator = (byte)(Candidate[(day.Fifty_Two_Week_Range - 1)]);

                profit = profit + CalculateProfit(day.Close, ref surgePot, nominator, NumberOfValues);
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

        private string BuildCompletion(int input)
        {
            string build = ("" + input);

            while(build.Length < 5)
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

        private long CalculateProfit(long close, ref long SurgePot, byte Nominator, int NumberOfValues)
        {
            int potFlow;

            Pot_Status_Is pot_status = CheckPot(SurgePot);

            potFlow = CalculatePotFlow(Nominator, NumberOfValues);

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

        private void CalculateIncrementer(int NumberOfValues, int BeginRange)
        {
            if (BeginRange < 0 || BeginRange >= 100)
            {
                throw new Exception(BeginRange + " was supplied for BeginRange argument for CalculateSurgeBuy... that number must be between 0 and 99");
            }

            int ranges = NumberOfValues - 1;

            if ((BeginRange * 10) % ((ranges * 10) / 2) != 0)
            {
                throw new Exception(NumberOfValues + " and " + BeginRange +
                                    " were supplied for NumberOfValues and for BeginRange arguments for CalculateSurgeBuy... " +
                                    "Check your math. (BeginRange) must be wholly divisible by ((NumberOfValues -1) / 2)");
            }

            this.incrementer = ((100 - BeginRange) / ranges) * 2;
        }

        private int CalculatePotFlow(byte nominator, int NumberOfValues)
        {
            int intermediate = ( (nominator * 10) - ( ( NumberOfValues * 10 ) / 2) );

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



    class SPY_Simulation_Result
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
