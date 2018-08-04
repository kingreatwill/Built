using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Built.Tool.CodeGenerator
{
    public class Class1
    {
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