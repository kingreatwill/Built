using System;
using System.IO;
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
            DirectoryMonitor monitor = new DirectoryMonitor(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            monitor.Change += (string filePath) =>
            {
                Console.WriteLine(filePath);
            };
            monitor.Start();
            Console.ReadLine();
        }
    }
}