using System;
using System.Threading;
using System.Threading.Tasks;

namespace Built.ConsoleApp1
{
    internal class Program
    {
        // proto文件队列;
        public static ProducerConsumer<string> ProtoQueue = new ProducerConsumer<string>(fileName =>
        {
            Console.WriteLine("1,正在消费" + fileName + "---" + Thread.CurrentThread.ManagedThreadId);
        });

        private static void Main(string[] args)
        {
            Console.WriteLine("1---" + Thread.CurrentThread.ManagedThreadId);

            for (var i = 0; ; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(10000);
                });
            }

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("3---" + Thread.CurrentThread.ManagedThreadId);
                for (var i = 0; ; i++)
                {
                    ProtoQueue.Enqueue("3-" + i.ToString());
                }
            });
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}