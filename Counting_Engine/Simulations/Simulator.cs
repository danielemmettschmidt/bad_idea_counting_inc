using System;
using Bad_Idea_Counting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Simulations
{
    public class Simulator
    {

        private Counting_Engine CE;

        public Simulator(int Number_of_Values, int Number_Of_Variables)
        {
            this.CE = new Counting_Engine(Number_of_Values, Number_Of_Variables);
        }

        public void RunSimulation<T>(int Number_Of_Threads, T Specific_Simulation)
        {
            List<SimThread<T>> threads = new List<SimThread<T>>();

            for (int ii = 0; ii < Number_Of_Threads; ii++)
            {

            }


        }
    }

    class SimThread<T>
    {

    }

}
