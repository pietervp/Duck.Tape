using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Duck.Tape.Test
{
    public class ConcreteClass
    {
        string ProtectedData { get; set; }
        void ProtectedMethod(){}

        event EventHandler ProtectedEvent;
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
}
