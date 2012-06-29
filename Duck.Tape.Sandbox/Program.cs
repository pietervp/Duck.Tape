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

    public class A 
    {
        public event EventHandler TestEvent;
        public event EventHandler<ConsoleCancelEventArgs> SecondEvent;

        public Tuple<string, int> Tuple { get; set; }
        public ITest Test { get; set; }

        public void Temp()
        {
        }

        public void Temp<T>()
        {
        }

        public void Temp<T>(T param)
        {
        }

        public void Temp(string x)
        {
        }

        public string ATemp()
        {
            return null;
        }

        public T ATemp<T>()
        {
            return default(T);
        }

        public T ATemp<T>(T param)
        {
            return default(T);
        }

        public int ATemp(string x)
        {
            return 0;
        }
    }
    
    public interface ITest
    {
        event EventHandler TestEvent;
        event EventHandler<ConsoleCancelEventArgs> SecondEvent;

        Tuple<string, int> Tuple { get; set; }
        ITest Test { get; set; }

        void Temp();
        void Temp<T>();
        void Temp<T>(T param);
        void Temp(string x);

        string ATemp();
        T ATemp<T>();
        T ATemp<T>(T param);
        int ATemp(string x);
    }
}
