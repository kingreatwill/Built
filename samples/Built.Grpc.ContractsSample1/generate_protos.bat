@rem Generate the C# code for .proto files

@rem enter this directory
cd /d %~dp0

set PROTOC=%UserProfile%\.nuget\packages\google.protobuf.tools\3.5.1\tools\windows_x64\protoc.exe
set PLUGIN=%UserProfile%\.nuget\packages\grpc.tools\1.9.0\tools\windows_x64\grpc_csharp_plugin.exe

dotnet restore

%PROTOC% -I protos --csharp_out gen_grpc_code  protos/helloworld.proto --grpc_out gen_grpc_code --plugin=protoc-gen-grpc=%PLUGIN%
%PROTOC% -I protos --csharp_out gen_grpc_code  protos/product_basic.proto --grpc_out gen_grpc_code --plugin=protoc-gen-grpc=%PLUGIN%