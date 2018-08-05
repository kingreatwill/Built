using Consul;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using static Grpc.Core.Server;

namespace Built.Grpc.Extensions
{
    public static class GrpcServiceRegisterExtension
    {
        public static Server StartAndRegisterConsul(this Server server)
        {
            server.Start();
            var ports = server.Ports;
            var srvNames = server.Services.GetServicesName();

            try
            {
                var client = new ConsulClient(p =>
                {
                    p.Address = new Uri("http://127.0.0.1:8500");
                }); 
                 client.Agent.ServiceRegister(new AgentServiceRegistration {
                    Address= "127.0.0.1",
                    Port= ports.First().BoundPort,
                    Name= srvNames.First(),
                    Tags= srvNames.ToArray(),
                    Check=new AgentServiceCheck {
                        //TTL=TimeSpan.FromSeconds(5),
                        Status = HealthStatus.Passing,
                        TCP= "127.0.0.1:50051",
                        Interval = TimeSpan.FromSeconds(5)
                    }
                 }).Wait();
              
                //client.Agent.CheckRegister(new AgentCheckRegistration
                //{
                //    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                //    TTL = TimeSpan.FromSeconds(ServiceConfig.TCPInterval),
                //    Status = HealthStatus.Passing,
                //    ID = ServiceConfig.GetConsulServiceId() + ":ttlcheck",
                //    ServiceID = ServiceConfig.GetConsulServiceId(),
                //    Name = "ttlcheck"
                //}).Wait();

                client.Dispose();
               

            }
            catch (Exception ex)
            {
                InnerLogger.Log(LoggerLevel.Error, $"consul Register failed {Environment.NewLine}{ex.ToString()}");
            }


            return server;
        }

        public static IEnumerable<string> GetServicesName(this ServiceDefinitionCollection serviceDefinitionCollection)
        {
            var result = new List<string>();
            foreach (var srv in serviceDefinitionCollection)
            {
                var sd = new ServiceDescriptor(typeof(ServerServiceDefinition), srv);
                var info = srv.GetType().GetField("callHandlers", BindingFlags.Instance | BindingFlags.NonPublic);

               
                foreach (var s in (IEnumerable)info.GetValue(srv))
                {
                    var kv = s.ToString().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    result.Add(kv[1]);
                    break;
                }
            }
            return result;
        }

    }
}
