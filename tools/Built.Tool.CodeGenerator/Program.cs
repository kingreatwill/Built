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
            var tree = CSharpSyntaxTree.ParseText(@" /// <summary> This is an xml doc comment </summary> class C { }");
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var classNode = (ClassDeclarationSyntax)(root.Members.First());
            var trivias = classNode.GetLeadingTrivia();
            var enumerator = trivias.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var trivia = enumerator.Current;
                if (trivia.Kind().Equals(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    var xml = trivia.GetStructure(); Console.WriteLine(xml);
                }
            }

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}