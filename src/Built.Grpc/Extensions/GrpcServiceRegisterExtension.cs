using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
