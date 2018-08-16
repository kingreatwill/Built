using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Built.Grpc.HttpGateway.Sample1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true);
            })
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.ConfigureAppConfiguration((hostingContext, config) => {
                //    var env = hostingContext.HostingEnvironment;
                //    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                //    config.AddEnvironmentVariables();
                //})
                //.ConfigureLogging((hostingContext, logging) =>
                //{
                //    var loggingCfig = hostingContext.Configuration.GetSection("Logging");
                //    logging.AddConfiguration(loggingCfig);
                //    logging.AddConsole();
                //    logging.AddDebug();
                //})
                //.ConfigureAppConfiguration(
                //       (hostingContext, builder) =>
                //       {
                //           builder

                //               .AddEnvironmentVariables();
                //       })
                .UseStartup<Startup>();
    }
}