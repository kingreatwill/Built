Reflection.Emit
SharpYaml
YamlDotNet



Dockerfile

FROM microsoft/aspnetcore:2.0.6

COPY bin/Release/netcoreapp2.0/publish /app
WORKDIR /app
 
EXPOSE 80/tcp
ENV ASPNETCORE_URLS http://*:5000
 
ENTRYPOINT ["dotnet", "Built.Extensions.Configuration.Consul.Test.Website.dll"]


docker-compose.yml

website:
  build: .
  dockerfile: Dockerfile
  links:
    - consul
  ports:
    - "80:5000"
consul:
  image: consul
  ports:
    - "8500:8500"