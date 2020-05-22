using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;

namespace Bad_Idea_Counting
{
    class Program
    {
        static void Main(string[] args)
        {
            Counting_Engine counting_engine = new Counting_Engine(10, 8, false);

        }
    }

}
