using System;
using Bad_Idea_Counting;
using SPY_Data_Processor;
using Simulations;

namespace SPY_Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = DirectoryFinder.GrabDir(args);

            string jsonFile = DirectoryValidator.DirectoryValidator.getfilename(dir, ".json");

            SPY_Simulation spy_sim = new SPY_Simulation(11, 8, 8, 50, 10, SPY_History.DeserializeFromJSONFile(jsonFile));



        }
    }
}
