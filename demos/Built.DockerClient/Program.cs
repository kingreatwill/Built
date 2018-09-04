using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Built.Docker
{
    /// <summary>
    /// 语言版本7.3
    /// </summary>
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            DockerClient client = new DockerClientConfiguration(new Uri("http://192.168.1.230:4243")).CreateClient();

            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
            new ContainersListParameters()
            {
                Limit = 10,
            });

            Console.WriteLine("Hello World!");
        }
    }
}