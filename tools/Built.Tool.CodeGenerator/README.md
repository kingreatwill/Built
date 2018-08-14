根据 proto 生成cs编译成dll
根据操作系统运行不同的文件
https://bbs.csdn.net/topics/360024896
https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-create-an-xml-documentation-file-using-codedom
1,csc
2,roslyn 跨平台
3,CSharpCodeProvider

E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\packages\google.protobuf.tools\3.5.1\tools\windows_x64\protoc  --csharp_out E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\temp_gen_grpc_code --proto_path=E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\protos helloworld.proto --grpc_out E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\temp_gen_grpc_code --plugin=protoc-gen-grpc=E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\packages\grpc.tools\1.9.0\tools\windows_x64\grpc_csharp_plugin.exe

var parseOptions = ParseOptions.Default.WithParseDocumentationComments(true); 
SyntaxTree unit = SyntaxTree.ParseFile(file, parseOptions);

using Roslyn.Compilers.CSharp; 
using System; 
using System.Linq; 
class Program { 
	static void Main(string[] args) 
	{ 
		var tree = SyntaxTree.ParseText(@" /// <summary>This is an xml doc comment</summary> class C { }"); 
		var classNode = (ClassDeclarationSyntax)tree.GetRoot().Members.First(); 
		var trivia = classNode.GetLeadingTrivia().Single(t => t.Kind == SyntaxKind.DocumentationCommentTrivia); 
		var xml = trivia.GetStructure(); Console.WriteLine(xml); 
		var compilation = Compilation.Create("test", syntaxTrees: new[] { tree }); 
		var classSymbol = compilation.GlobalNamespace.GetTypeMembers("C").Single(); 
		var docComment = classSymbol.GetDocumentationComment(); 
		Console.WriteLine(docComment.SummaryTextOpt); 
	}
}

using System; 
using System.Linq; 
using Microsoft.CodeAnalysis.CSharp; 
using Microsoft.CodeAnalysis.CSharp.Syntax; 
class Program { 
static void Main(string[] args) { 
		var tree = CSharpSyntaxTree.ParseText(@" /// <summary> This is an xml doc comment </summary> class C { }"); 
		var root = (CompilationUnitSyntax) tree.GetRoot(); 
		var classNode = (ClassDeclarationSyntax) (root.Members.First()); 
		var trivias = classNode.GetLeadingTrivia(); 
		var xmlCommentTrivia = trivias.FirstOrDefault(t => t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia); 
		var xml = xmlCommentTrivia.GetStructure(); 
		Console.WriteLine(xml); 
		var compilation = CSharpCompilation.Create("test", syntaxTrees: new[] {tree}); 
		var classSymbol = compilation.GlobalNamespace.GetTypeMembers("C").Single(); 
		var docComment = classSymbol.GetDocumentationCommentXml(); 
		Console.WriteLine(docComment); 
	} 
}

using System; 
using System.Linq; 
using Microsoft.CodeAnalysis.CSharp; 
using Microsoft.CodeAnalysis.CSharp.Syntax; 
class Program { 
	static void Main(string[] args) { 
		var tree = CSharpSyntaxTree.ParseText(@" /// <summary> This is an xml doc comment </summary> class C { }"); 
		var root = (CompilationUnitSyntax) tree.GetRoot(); 
		var classNode = (ClassDeclarationSyntax) (root.Members.First()); 
		var trivias = classNode.GetLeadingTrivia(); 
		var enumerator = trivias.GetEnumerator(); 
		while (enumerator.MoveNext()) { 
			var trivia = enumerator.Current; 
			if(trivia.Kind().Equals(SyntaxKind.SingleLineDocumentationCommentTrivia)) 
			{ 
				var xml = trivia.GetStructure(); Console.WriteLine(xml);
			} 
		} 
	} 
}


