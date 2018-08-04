根据 proto 生成cs编译成dll
根据操作系统运行不同的文件
https://bbs.csdn.net/topics/360024896
1,csc
2,roslyn 跨平台
3,CSharpCodeProvider

E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\packages\google.protobuf.tools\3.5.1\tools\windows_x64\protoc  --csharp_out E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\temp_gen_grpc_code --proto_path=E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\protos helloworld.proto --grpc_out E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\temp_gen_grpc_code --plugin=protoc-gen-grpc=E:\Built\tools\Built.Tool.CodeGenerator\bin\Debug\netcoreapp2.1\packages\grpc.tools\1.9.0\tools\windows_x64\grpc_csharp_plugin.exe