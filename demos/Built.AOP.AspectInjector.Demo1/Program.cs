using System;
using System.Diagnostics;
using System.Threading;

//using AspectInjector.Broker;
//using AspectInjector.Samples.Logging.Aspects;

namespace Built.AOP.AspectInjector.Demo1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var service = new SampleService();
            //service.GetCount();
            Console.WriteLine("Hello World!");
        }
    }

    //[Aspect(Aspect.Scope.Global)]
    //internal class LoggingAspect
    //{
    //    [Advice(Advice.Type.Around, Advice.Target.Method)]
    //    public object HandleMethod([Advice.Argument(Advice.Argument.Source.Name)] string name,
    //        [Advice.Argument(Advice.Argument.Source.Arguments)] object[] arguments,
    //        [Advice.Argument(Advice.Argument.Source.Target)] Func<object[], object> method)
    //    {
    //        Console.WriteLine($"Executing method {name}");

    //        var sw = Stopwatch.StartNew();

    //        var result = method(arguments);

    //        sw.Stop();

    //        Console.WriteLine($"Executed method {name} in {sw.ElapsedMilliseconds} ms");

    //        return result;
    //    }
    //}

    //[Inject(typeof(LoggingAspect))]
    //public class SampleService
    //{
    //    public int GetCount()
    //    {
    //        Thread.Sleep(3000);

    //        return 10;
    //    }
    //}
}