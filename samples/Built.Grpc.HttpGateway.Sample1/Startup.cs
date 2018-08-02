using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true)
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                ;
            Configuration = builder.Build();
            var sd = Configuration.GetValue<string>("datasource:driver-class-name");
            var srvConfig = Configuration.
               GetSection("datasource").Get<DatasourceConfig>();
            // Configuration = configuration;
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
            //http://localhost:5000/ProductBasic.ProductBasicSrv/Gets?PageIndex=1&PageSize=10
            var pipeline = new PipelineBuilder()
                .Use(async (ctx, next) =>
                {
                    Console.WriteLine($"--------Request--------{ctx.Request.ToString()}-------------------");
                    await next(ctx);
                    Console.WriteLine($"--------Response--------{ctx.Response.ToString()}-------------------");
                });
            app.UseGrpcHttpGateway(pipeline.Build());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}