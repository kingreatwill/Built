using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Built.Grpc.HttpGateway.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;

namespace Built.Grpc.HttpGateway.Sample1
{
    public class DatasourceConfig
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // [YamlMember(Alias = "driver-class-name")]
        public string DriverClassName { get; set; }
    }

    /// <summary>
    /// 你好啊
    /// </summary>
    public class Startup
    {
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-2.1
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            //var builderJson = new ConfigurationBuilder()
            //   .SetBasePath(env.ContentRootPath)
            //   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            ////.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            ////.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //var ConfigurationJson = builderJson.Build();

            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true);
            //configuration = builder.Build();
            //configuration.
            //var sd = Configuration.GetValue<string>("datasource:driver-class-name");
            //var srvConfig = Configuration.
            //   GetSection("datasource").Get<DatasourceConfig>();

            InnerLogger.ConsulLog += (obj) =>
            {
                Console.WriteLine(obj.Content);
            };
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine($"--------Configure-------------------");
            //http://localhost:5000/ProductBasic.ProductBasicSrv/Gets?PageIndex=1&PageSize=10
            var pipeline = new PipelineBuilder()
                .Use(async (ctx, next) =>
                {
                    Console.WriteLine($"--------Request--------{ctx.Request.ToString()}-------------------");
                    await next(ctx);
                    Console.WriteLine($"--------Response--------{ctx.Response.ToString()}-------------------");
                });
            app.UseBuiltGrpcSwagger(new SwaggerOptions("Built.Grpc.HttpGateway.Sample1", "GRPC 文档", "/")
            {
                // XmlDocumentPath = xmlPath
            });
            app.UseGrpcHttpGateway(pipeline.Build())
                .UseGrpcMonitorDllFileEnable()
                .UseGrpcMonitorProtoFileEnable();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // app.UseStaticFiles();
            app.UseMvc();
        }
    }
}