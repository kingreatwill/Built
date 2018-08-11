using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Built.Tool.CodeGenerator
{
    public class Class1
    {

        void Grpc()
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
        }
        //public string CompileCode()
        //{
        //    var syntaxTree = CSharpSyntaxTree.ParseText(source);

        //    CSharpCompilation compilation = CSharpCompilation.Create(
        //        "assemblyName",
        //        new[] { syntaxTree },
        //        new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
        //        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        //    using (var dllStream = new MemoryStream())
        //    using (var pdbStream = new MemoryStream())
        //    {
        //        var emitResult = compilation.Emit(dllStream, pdbStream);
        //        if (!emitResult.Success)
        //        {
        //            // emitResult.Diagnostics
        //        }
        //    }
        //}

        public void sd()
        {
            //ソースコードをParseしてSyntax Treeを生成
            var sourceTree = CSharpSyntaxTree.ParseText("");

            //コードが参照するアセンブリの一覧
            var references = new[]{
                //microlib.dll
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                //System.dll
               MetadataReference.CreateFromFile(typeof(System.Collections.Generic.AsyncEnumerator).Assembly.Location),
                //System.Core.dll
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                //ConsoleApplication1.exe
                MetadataReference.CreateFromFile(typeof(Program).Assembly.Location),
            };

            //コンパイルオブジェクト
            var compilation = CSharpCompilation.Create("GeneratedAssembly",
                new[] { sourceTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            Assembly assembly = null;
            using (var stream = new System.IO.MemoryStream())
            {
                //コンパイル実行。結果をストリームに書き込む
                var result = compilation.Emit(stream);
                if (result.Success)
                {
                    //成功時 アセンブリとしてロードする。
                    assembly = System.Reflection.Assembly.Load(stream.GetBuffer());
                }
                else
                {
                    //失敗時 コンパイルエラーを出力
                    //foreach (var mes in result.Diagnostics.Select(d =>
                    //    string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                    //{
                    //    Console.Error.WriteLine(mes);
                    //}
                    //return null;
                }
            }

            //アセンブリから目的のクラスを取得してインスタンスを生成
            var type = assembly.GetType("ConsoleApplication1.GeneratedExecutor");
            // return (ICodeExecutor)Activator.CreateInstance(type);
        }
    }
}