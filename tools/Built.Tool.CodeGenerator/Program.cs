using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Built.Tool.CodeGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var architecture = RuntimeInformation.OSArchitecture.ToString().ToLower();// 系统架构,x86 x64
            var os = "windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "macosx";
            }

            var protoc = $@"packages\google.protobuf.tools\3.5.1\tools\{os}_{architecture}\protoc";//windows  exe
            var protocPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, protoc);
            // grpc_csharp_plugin
            var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"packages\grpc.tools\1.9.0\tools\{os}_{architecture}\grpc_csharp_plugin.exe");
            var protoc_args = $" --proto_path={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "protos")} --csharp_out {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_gen_grpc_code")} helloworld.proto --grpc_out {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_gen_grpc_code")} --plugin=protoc-gen-grpc={pluginPath}";
            var psi = new ProcessStartInfo(protocPath, protoc_args)
            {
                RedirectStandardOutput = true
            };
            //启动
            var proc = Process.Start(psi);
            if (proc == null)
            {
                Console.WriteLine("Can not exec.");
            }
            else
            {
                Console.WriteLine("-------------Start read standard output--------------");
                //开始读取
                using (var sr = proc.StandardOutput)
                {
                    while (!sr.EndOfStream)
                    {
                        Console.WriteLine(sr.ReadLine());
                    }

                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }
                }
                Console.WriteLine("---------------Read end------------------");
                Console.WriteLine($"Total execute time :{(proc.ExitTime - proc.StartTime).TotalMilliseconds} ms");
                Console.WriteLine($"Exited Code ： {proc.ExitCode}");
            }

            Console.WriteLine("Hello World!");
        }
    }
}