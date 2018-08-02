using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
    }
}