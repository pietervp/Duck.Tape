using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Duck.Tape.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new A();

            for (int i = 0; i < 10; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                var duckx = a.Duck<ITest>();
                Console.WriteLine("{0} ms", stopwatch.ElapsedMilliseconds);
            }

            var duck = a.Duck<ITest>();
            var duck1 = a.Duck<ITest>();
            var duck2 = a.Duck<ITest>();
            var duck3 = a.Duck<ITest>();

            duck.TestEvent += DuckOnTestEvent;
            duck.TestEvent -= DuckOnTestEvent;
            
            duck.SecondEvent += DuckOnSecondEvent;
            duck.SecondEvent -= DuckOnSecondEvent;

            duck.Temp();
            duck.Temp("");
            duck.Temp<int>();
            duck.Temp(77);
            duck.ATemp();
            duck.ATemp("");
            duck.ATemp<ITest>();
            duck.ATemp("pieter");

            duck.Test = new A().Duck<ITest>();
            duck.Tuple= new Tuple<string, int>("", 1);

            Console.Read();
        }

        private static void DuckOnSecondEvent(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Console.WriteLine("second event");
        }

        private static void DuckOnTestEvent(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Event");
        }
    }

}
