using System;
using Bad_Idea_Counting;
using SPY_Data_Processor;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPY_Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = DirectoryFinder.GrabDir(args);

            string jsonFile = DirectoryValidator.DirectoryValidator.getfilename(dir, ".json");

            SPY_Simulation spy_sim = new SPY_Simulation(11, 8, 6, 50, 10, SPY_History.DeserializeFromJSONFile(jsonFile));

            Console.WriteLine("COMPLETE:");

            string dump = Console.ReadLine();

            File.WriteAllText((dir + "\\" + DateTime.UtcNow.ToString("MM_dd_yyyy_HH_mm_ss") + "_" + dump+ ".json"), JsonConvert.SerializeObject(spy_sim));

        }
    }
}
