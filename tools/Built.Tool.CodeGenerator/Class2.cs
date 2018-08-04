using Grpc.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Text;

namespace Built.Tool.CodeGenerator
{
    internal class Class2
    {
        public void test2()
        {
            string csPaths = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_gen_grpc_code");
            var dllFiles = Directory.GetFiles(csPaths, "*.cs");
            //ソースコードをParseしてSyntax Treeを生成
            var sourceTree1 = CSharpSyntaxTree.ParseText(File.ReadAllText(dllFiles[0]));
            var sourceTree2 = CSharpSyntaxTree.ParseText(File.ReadAllText(dllFiles[1]));

            //コードが参照するアセンブリの一覧
            var references = new[]{
                //microlib.dll
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.IO.Stream).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Threading.CancellationToken).Assembly.Location),

                /*

                 using System;
using System.Threading;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;
                 */

                //Google.Protobuf.dll
                MetadataReference.CreateFromFile(typeof(Google.Protobuf.Reflection.ServiceDescriptor).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Grpc.Core.ClientBase).Assembly.Location),
            };

            /*

              COM.ReferencedAssemblies.Add("mscorlib.dll");
            COM.ReferencedAssemblies.Add("System.dll");
            COM.ReferencedAssemblies.Add("System.Interactive.Async.dll");
            COM.ReferencedAssemblies.Add(@"Google.Protobuf.dll");
            COM.ReferencedAssemblies.Add(@"Grpc.Core.dll");
             */

            var compilation = CSharpCompilation.Create("GeneratedAssembly",
                new[] { sourceTree1, sourceTree2 },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            Assembly assembly = null;
            using (var stream = new System.IO.MemoryStream())
            {
                var result = compilation.Emit(stream);
                if (result.Success)
                {
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
        }

        public void sd()
        {
            // CodeDom已被Roslyn API取代。目前.net core平台不支持CodeDom
            string csPaths = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_gen_grpc_code");
            var dllFiles = Directory.GetFiles(csPaths, "*.cs");
            string strDll = "GrpcSrvTemp.dll";

            CodeDomProvider COD = new Microsoft.CSharp.CSharpCodeProvider();
            COD = new Microsoft.CSharp.CSharpCodeProvider();
            CompilerParameters COM = new CompilerParameters
            {
                //生成DLL，True为生成exe文件,false为生成dll文件
                GenerateExecutable = false,
                OutputAssembly = strDll,
            };
            COM.ReferencedAssemblies.Add("mscorlib.dll");
            COM.ReferencedAssemblies.Add("System.dll");
            COM.ReferencedAssemblies.Add("System.Interactive.Async.dll");
            COM.ReferencedAssemblies.Add(@"Google.Protobuf.dll");
            COM.ReferencedAssemblies.Add(@"Grpc.Core.dll");
            CompilerResults COMR = COD.CompileAssemblyFromFile(COM, dllFiles);

            //下面我们就可以根据生成的Dll反射为相关对象，供我们使用了．
            //Assembly a = Assembly.LoadFrom(strDll);
            //Type t = a.GetType("b");
            //object obj = Activator.CreateInstance(t);
            //t.GetMethod("run").Invoke(obj, null);
        }
    }
}