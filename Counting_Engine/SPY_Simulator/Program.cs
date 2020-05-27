using System;
using Bad_Idea_Counting;
using SPY_Data_Processor;

namespace SPY_Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Counting_Engine CE = new Counting_Engine(10, 8);

            string dir = DirectoryFinder.GrabDir(args);

            string jsonFile = DirectoryValidator.DirectoryValidator.getfilename(dir, ".json");

            SPY_History SPY = SPY_History.DeserializeFromJSONFile(jsonFile);



        }
    }
}
