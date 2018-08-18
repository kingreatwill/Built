using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
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
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-2.1
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            //http://www.cnblogs.com/yilezhu/p/9297009.html
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

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var audienceConfig = Configuration.GetSection("JwtAuthorize");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],

                // Validate the token expiry
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //不使用https
                //o.RequireHttpsMetadata = false;
                //o.TokenValidationParameters = tokenValidationParameters;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = audienceConfig["Audience"],
                    IssuerSigningKey = signingKey
                };
            });

            // services.AddAuthentication(x =>
            // {
            //     x.DefaultScheme = "Bearer";
            // })
            //.AddJwtBearer("Bearer", o =>
            //{
            //    o.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
            //        //ValidateIssuer = false, // 默认为true
            //        //ValidateAudience = false, // 默认为true

            //        //ValidIssuer = "http://localhost:5200",
            //        //ValidAudience = "api",
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("12315sdfsdfsgscd@"))
            //    };
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Built.Grpc.HttpGateway.Sample1.xml"));
                options.SwaggerDoc("Gateway",
                    new Info
                    {
                        Title = "GRPC网关服务",
                        Version = "v1",
                        Contact = new Contact
                        {
                            Email = "350840291@qq.com",
                            Name = "Swagger",
                            Url = "http://127.0.0.1"
                        },
                        Description = "GRPC网关服务描述"
                    });
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "请输入带有Bearer的Token", Name = "Authorization", Type = "apiKey" });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {
                        "Bearer",
                        Enumerable.Empty<string>()
                    }
                });
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
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
            app.UseBuiltGrpcSwagger(new Swagger.SwaggerOptions("Built.GrpcGateway", "GRPC 文档", "/", "/swagger/grpc/swagger.json")
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
            //var audienceConfig = Configuration.GetSection("Audience");
            //var symmetricKeyAsBase64 = audienceConfig["Secret"];
            //var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            //var signingKey = new SymmetricSecurityKey(keyByteArray);
            //var SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            //https://www.cnblogs.com/hzzxq/p/7373287.html
            app.UseAuthentication();
            app.UseSwagger()
              .UseSwaggerUI(options =>
              {
                  options.SwaggerEndpoint("/swagger/grpc/swagger.json", "grpc 网关");//grpc 网关
                  options.SwaggerEndpoint("/swagger/Gateway/swagger.json", "webapi");//webapi
                  options.SwaggerEndpoint($"/a/swagger.json", $"A");
                  options.SwaggerEndpoint($"/b/swagger.json", $"B");
                  options.DocumentTitle = "Swagger测试平台";
              });
            app.UseMvc();
        }
    }
}