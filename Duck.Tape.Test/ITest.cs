using System;

namespace Duck.Tape.Test
{
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