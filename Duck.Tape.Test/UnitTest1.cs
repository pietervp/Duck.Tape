using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Duck.Tape.Test
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void DuckedTypeIsInstanceOfInterface()
        {
            var duck = new A().Duck<ITest>();

            Assert.IsInstanceOfType(duck, typeof(ITest));
        }

        [TestMethod]
        public void DuckedTypeIsNotNull()
        {
            var duck = new A().Duck<ITest>();

            Assert.IsNotNull(duck);
        }

        [TestMethod]
        [ExpectedException(typeof(DuckedMemberNotFoundException))]
        public void DuckedTypeCannotGetPrivateProperty()
        {
            var a = new A();
            var duck = a.Duck<IProtectedInterface>();

            duck.ProtectedData = "test";
        }

        [TestMethod]
        [ExpectedException(typeof(DuckedMemberNotFoundException))]
        public void DuckedTypeCannotGetPrivateMethod()
        {
            var a = new A();
            var duck = a.Duck<IProtectedInterfaceWithMethod>();

            duck.ProtectedMethod();
        }

        [TestMethod]
        [ExpectedException(typeof(DuckedMemberNotFoundException))]
        public void DuckedTypeCannotGetPrivateEvent()
        {
            var a = new A();
            var duck = a.Duck<IProtectedInterfaceWithEvent>();

            duck.ProtectedEvent += (sender, args) => args.ToString();
        }

        [TestMethod]
        public void DuckedTypeCanGetProperty()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            a.Tuple = new Tuple<string, int>("test", 1);

            Assert.AreEqual(a.Tuple.Item1, duck.Tuple.Item1);
        }

        [TestMethod]
        public void DuckedTypeCanSetProperty()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            a.Tuple = new Tuple<string, int>("test", 1);
            duck.Tuple = new Tuple<string, int>("test2", 1);

            Assert.AreEqual(a.Tuple.Item1, "test2");
        }

        [TestMethod]
        public void DuckedTypeCallNonReturnMethod()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.Temp();
        }

        [TestMethod]
        public void DuckedTypeCallNonReturnGenericMethod()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.Temp<string>();
        }
        [TestMethod]
        public void DuckedTypeCallNonReturnGenericMethodWithParameters()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.Temp<string>("");
        }
        [TestMethod]
        public void DuckedTypeCallNonReturnMethodWithParameters()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.Temp("");
        }
        
        [TestMethod]
        public void DuckedTypeCallReturnMethod()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.ATemp();
        }

        [TestMethod]
        public void DuckedTypeCallReturnGenericMethod()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.ATemp<string>();
        }

        [TestMethod]
        public void DuckedTypeCallReturnGenericMethodWithParameters()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.ATemp<string>("");
        }

        [TestMethod]
        public void DuckedTypeCallReturnMethodWithParameters()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.ATemp("");
        }
        
        [TestMethod]
        public void DuckedTypeCanAddEventHandler()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.TestEvent += (sender, args) => { };
        }

        [TestMethod]
        public void DuckedTypeCanAddAndRemoveEventHandler()
        {
            var a = new A();
            var duck = a.Duck<ITest>();
            duck.TestEvent += DuckOnTestEvent;
            duck.TestEvent -= DuckOnTestEvent;
        }

        private void DuckOnTestEvent(object sender, EventArgs eventArgs)
        {
            
        }
    }

    public interface IProtectedInterfaceWithEvent
    {
        event EventHandler ProtectedEvent;
    }

    public interface IProtectedInterfaceWithMethod
    {
        void ProtectedMethod();
    }

    public interface IProtectedInterface
    {
        string ProtectedData { get; set; }
    }
    
    public class A
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
