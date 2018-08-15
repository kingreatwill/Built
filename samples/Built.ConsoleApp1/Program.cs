using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Built.ConsoleApp1
{
    internal class Program
    {
        // proto文件队列;
        public static ProducerConsumer<string> ProtoQueue = new ProducerConsumer<string>(fileName =>
        {
            var architecture = RuntimeInformation.OSArchitecture.ToString().ToLower();// 系统架构,x86 x64
            var bin = string.Empty;
            var os = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "windows";
                bin = ".exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "macosx";
            }
            else
            {
                Console.WriteLine("该平台不支持.");
            }

            Console.WriteLine("1,正在消费" + fileName + "---" + Thread.CurrentThread.ManagedThreadId + "--------os-" + os + "------------" + architecture + "----");
        });

        private static void Main(string[] args)
        {
            Console.WriteLine("Enter exit to exit!");
            var cl = Console.ReadLine();
            do
            {
                ProtoQueue.Enqueue(cl);
                cl = Console.ReadLine();
            } while (cl != "exit");
        }
    }
}