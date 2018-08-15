using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Built.Tool.CodeGenerator
{
    internal class Program
    {
        public static string ReadFileString(string path)
        {
            // Use StreamReader to consume the entire text file.
            using (StreamReader reader = new StreamReader(path, encoding: Encoding.GetEncoding("GB2312")))
            {
                return reader.ReadToEnd();
            }
        }

        private static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sd = @"E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\ProductBasic.cs";
            var s = ReadFileString(sd);// File.ReadAllText(sd, encoding: Encoding.Default);

            var sd2 = @"E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\ProductBasic1.cs";
            var s2 = ReadFileString(sd2);// File.ReadAllText(sd, encoding: Encoding.Default);
            //    var tree = CSharpSyntaxTree.ParseText(@" /// <summary> 你好啊 </summary>
            //class C { }");
            //    var root = (CompilationUnitSyntax)tree.GetRoot();
            //    var classNode = (ClassDeclarationSyntax)(root.Members.First());
            //    var trivias = classNode.GetLeadingTrivia();
            //    var xmlCommentTrivia = trivias.FirstOrDefault(t => t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);
            //    var xml = xmlCommentTrivia.GetStructure();
            //    Console.WriteLine(xml);
            //    var compilation = CSharpCompilation.Create("test", syntaxTrees: new[] { tree });
            //    var classSymbol = compilation.GlobalNamespace.GetTypeMembers("C").Single();
            //    var docComment = classSymbol.GetDocumentationCommentXml();
            //    Console.WriteLine(docComment);

            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}