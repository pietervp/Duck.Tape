using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Duck.Tape;

namespace Duck.Tape.PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var startNew = Stopwatch.StartNew();
            TestCastle();
            Console.WriteLine(startNew.ElapsedMilliseconds);
            startNew.Restart();
            TestDuckTape();
            Console.WriteLine(startNew.ElapsedMilliseconds);
            startNew.Restart();
            TestLinFu();
            Console.WriteLine(startNew.ElapsedMilliseconds);
        }

        public static void TestDuckTape()
        {
            var target = new A();

            for (int i = 0; i < 1000; i++)
            {
                var targetInterface = target.Duck<ITest>();
                targetInterface.Temp();
            }
        }

        public static void TestLinFu()
        {
            var proxyGenerator = new LinFu.DynamicProxy.ProxyFactory();
            
            for (int i = 0; i < 1000; i++)
            {
                var targetInterface = proxyGenerator.CreateProxyType(typeof(A), typeof(ITest)).GetConstructors()[0].Invoke(null) as ITest;
                //targetInterface.Temp();
            }
        }

        public static void TestCastle()
        {
            var proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
            var target = new A();

            for (int i = 0; i < 1000; i++)
            {
                //var targetInterface = proxyGenerator.crea
            }
        }
    }

    public class DuckTypingInterceptor : IInterceptor
    {
        private readonly object target;
        private readonly Dictionary<KeyValuePair<MethodInfo, Type[]>, MethodInfo> methods = new Dictionary<KeyValuePair<MethodInfo, Type[]>, MethodInfo>();

        public DuckTypingInterceptor(object target, Type targetType)
        {
            this.target = target;
            BuildMethodDictionary(target.GetType(), targetType);
        }

        public void BuildMethodDictionary(Type type, Type targetType)
        {
            //type.GetMethods()[0].GetGenericArguments()
            //throw new NotImplementedException();
        }

        public void Intercept(IInvocation invocation)
        {
            var methods = target.GetType().GetMethods()
                .Where(m => m.Name == invocation.Method.Name)
                .Where(m => m.GetParameters().Length == invocation.Arguments.Length)
                .ToList();
            if (methods.Count > 1)
                throw new ApplicationException(string.Format("Ambiguous method match for '{0}'", invocation.Method.Name));
            if (methods.Count == 0)
                throw new ApplicationException(string.Format("No method '{0}' found", invocation.Method.Name));
            var method = methods[0];
            if (invocation.GenericArguments != null && invocation.GenericArguments.Length > 0)
                method = method.MakeGenericMethod(invocation.GenericArguments);
            invocation.ReturnValue = method.Invoke(target, invocation.Arguments);
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
