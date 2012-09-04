using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Duck.Tape.Test
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void DuckedTypeIsInstanceOfInterface()
        {
            var duck = new ConcreteClass().Duck<ITest>();

            Assert.IsInstanceOfType(duck, typeof(ITest));
        }

        [TestMethod]
        public void DuckedTypeIsNotNull()
        {
            var duck = new ConcreteClass().Duck<ITest>();

            Assert.IsNotNull(duck);
        }

        [TestMethod]
        [ExpectedException(typeof(DuckedMemberNotFoundException))]
        public void DuckedTypeCannotGetPrivateProperty()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<IProtectedInterface>();

            duck.ProtectedData = "test";
        }

        [TestMethod]
        [ExpectedException(typeof(DuckedMemberNotFoundException))]
        public void DuckedTypeCannotGetPrivateMethod()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<IProtectedInterfaceWithMethod>();

            duck.ProtectedMethod();
        }

        [TestMethod]
        [ExpectedException(typeof(DuckedMemberNotFoundException))]
        public void DuckedTypeCannotGetPrivateEvent()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<IProtectedInterfaceWithEvent>();

            duck.ProtectedEvent += (sender, args) => args.ToString();
        }

        [TestMethod]
        public void DuckedTypeCanGetProperty()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            a.Tuple = new Tuple<string, int>("test", 1);

            Assert.AreEqual(a.Tuple.Item1, duck.Tuple.Item1);
        }

        [TestMethod]
        public void DuckedTypeCanSetProperty()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            a.Tuple = new Tuple<string, int>("test", 1);
            duck.Tuple = new Tuple<string, int>("test2", 1);

            Assert.AreEqual(a.Tuple.Item1, "test2");
        }

        [TestMethod]
        public void DuckedTypeCallNonReturnMethod()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.Temp();
        }

        [TestMethod]
        public void DuckedTypeCallNonReturnGenericMethod()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.Temp<string>();
        }
        [TestMethod]
        public void DuckedTypeCallNonReturnGenericMethodWithParameters()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.Temp<string>("");
        }
        [TestMethod]
        public void DuckedTypeCallNonReturnMethodWithParameters()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.Temp("");
        }
        
        [TestMethod]
        public void DuckedTypeCallReturnMethod()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.ATemp();
        }

        [TestMethod]
        public void DuckedTypeCallReturnGenericMethod()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.ATemp<string>();
        }

        [TestMethod]
        public void DuckedTypeCallReturnGenericMethodWithParameters()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.ATemp<string>("");
        }

        [TestMethod]
        public void DuckedTypeCallReturnMethodWithParameters()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.ATemp("");
        }
        
        [TestMethod]
        public void DuckedTypeCanAddEventHandler()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.TestEvent += (sender, args) => { };
        }

        [TestMethod]
        public void DuckedTypeCanAddAndRemoveEventHandler()
        {
            var a = new ConcreteClass();
            var duck = a.Duck<ITest>();
            duck.TestEvent += DuckOnTestEvent;
            duck.TestEvent -= DuckOnTestEvent;
        }

        private void DuckOnTestEvent(object sender, EventArgs eventArgs)
        {
            
        }
    }
}