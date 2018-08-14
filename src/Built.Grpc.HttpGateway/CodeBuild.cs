using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using grpc = global::Grpc.Core;

namespace Built.Grpc.HttpGateway
{
    public class CodeBuild
    {
        public static bool Build(string csPath, string assemblyName)
        {
            var dllFiles = Directory.GetFiles(csPath, "*.cs");
            if (dllFiles.Length == 0) return false;
            List<SyntaxTree> trees = new List<SyntaxTree>();
            foreach (var file in dllFiles)
            {
                trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(file)));
            }
            var references2 = new[]{
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=0.0.0.0").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.IO, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Threading.Tasks, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Google.Protobuf.ProtoPreconditions).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(grpc.Channel).Assembly.Location),
            };
            var options = new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);
            var compilation = CSharpCompilation.Create(assemblyName, trees, references2, options);
            var result2 = compilation.Emit(Path.Combine(csPath, $"{assemblyName}.dll"), xmlDocPath: Path.Combine(csPath, $"{assemblyName}.xml"));

            InnerLogger.Log(
                result2.Success ? LoggerLevel.Debug : LoggerLevel.Error,
                string.Join(",", result2.Diagnostics.Select(d => string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                );
            Thread.Sleep(100);
            return result2.Success;
        }
    }
}