using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Built.Grpc.HttpGateway
{
    public class CodeGenerate
    {
        public static bool Generate(string baseDirectory, string protoFile)
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
                InnerLogger.Log(LoggerLevel.Error, "该平台不支持.");
                return false;
            }
            var protocBinPath = Path.Combine(baseDirectory, $"tools/{os}_{architecture}/protoc{bin}");
            var pluginBinPath = Path.Combine(baseDirectory, $"tools/{os}_{architecture}/grpc_csharp_plugin{bin}");
            var csharp_out = Path.Combine(baseDirectory, $"plugins/.{Path.GetFileNameWithoutExtension(protoFile)}");
            // 创建文件夹
            if (!Directory.Exists(csharp_out)) Directory.CreateDirectory(csharp_out);
            //protoFile.GetMD5();

            var proto_path = Path.Combine(baseDirectory, "protos");
            var protoc_args = $" --proto_path={proto_path} --csharp_out {csharp_out} {Path.GetFileName(protoFile)} --grpc_out {csharp_out} --plugin=protoc-gen-grpc={pluginBinPath}";
            Console.WriteLine(protocBinPath + "     " + protoc_args);
            var psi = new ProcessStartInfo(protocBinPath, protoc_args)
            {
                RedirectStandardOutput = true
            };
            //启动
            using (var proc = System.Diagnostics.Process.Start(psi))
            {
                if (proc == null)
                {
                    InnerLogger.Log(LoggerLevel.Debug, "-------------Can not exec.--------------");
                    return false;
                }
                else
                {
                    var output = proc.StandardOutput.ReadToEnd();

                    InnerLogger.Log(LoggerLevel.Debug, "-------------Start read standard output--------------");
                    InnerLogger.Log(LoggerLevel.Debug, "-------------" + output + "--------------");
                    ////开始读取
                    //using (var sr = proc.StandardOutput)
                    //{
                    //    while (!sr.EndOfStream)
                    //    {
                    //        InnerLogger.Log(LoggerLevel.Debug, sr.ReadLine());
                    //    }

                    //    if (!proc.HasExited)
                    //    {
                    //        proc.Kill();
                    //    }
                    //}
                    InnerLogger.Log(LoggerLevel.Debug, "---------------Read end------------------");
                    InnerLogger.Log(LoggerLevel.Debug, $"Exited Code ： {proc.ExitCode}");
                }
                Thread.Sleep(100);
            }
            return true;
        }
    }
}