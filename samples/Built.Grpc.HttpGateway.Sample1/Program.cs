using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Built.Grpc.HttpGateway.Sample1
{
    public class Program
    {
        /*
         kill -l
         2.   SIGNIT  (Ctrl+C)

3.   SIGQUIT （退出）

9.   SIGKILL   (强制终止）

15. SIGTERM （终止）

            main()
var cts = new CancellationTokenSource();
var bgtask = Task.Run(() => { TestService.Run(cts.Token); });
AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                  Console.WriteLine($"{DateTime.Now} 后台测试服务，准备进行资源清理！");

                 cts.Cancel();    //设置IsCancellationRequested=true，让TestService今早结束
                 bgtask.Wait();   //等待 testService 结束执行

                 Console.WriteLine($"{DateTime.Now} 恭喜，Test服务程序已正常退出！");

                 Environment.Exit(0);
             };
             Console.WriteLine($"{DateTime.Now} 后端服务程序正常启动！");
             bgtask.Wait();
             */

        public static void Main(string[] args)
        {
            //AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static readonly CancellationTokenSource ConfigCancellationTokenSource = new CancellationTokenSource();
        //private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

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