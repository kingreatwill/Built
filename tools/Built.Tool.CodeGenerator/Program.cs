using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Built.Tool.CodeGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string csPaths = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_gen_grpc_code");
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(csPaths, "Helloworld.txt")));
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var classNode = (ClassDeclarationSyntax)(root.Members.First());
            var trivias = classNode.GetLeadingTrivia();
            var enumerator = trivias.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var trivia = enumerator.Current;
                if (trivia.Kind().Equals(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    var xml = trivia.GetStructure();
                    Console.WriteLine(xml);
                }
            }

            ////Helloworld.txt

            //CSharpSyntaxTree.ParseText(@" /// <summary> This is an xml doc comment </summary>
            //    class C { }");
            //var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(csPaths, "Helloworld.txt")));

            //var compilation = CSharpCompilation.Create("test", syntaxTrees: new[] { tree });
            //var classSymbol = compilation.GlobalNamespace.GetNamespaceMembers();
            //var df = compilation.GlobalNamespace.GetTypeMembers("HelloworldReflection");
            //// var docComment = classSymbol.GetDocumentationCommentXml();

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}