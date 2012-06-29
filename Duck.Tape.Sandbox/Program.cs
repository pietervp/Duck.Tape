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
        A a = new A();
        ITest aAsITest = a.Duck<ITest>();
    }
}

public class A
{
    string Data { get; set; }

    public void Method()
    {
            
    }
}
    
public interface ITest
{
    string Data { get; set; }
    void Method();
}
}
