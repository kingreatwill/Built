using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Built.Micro.ImageCloud.Domain.Services;
using Built.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Memory;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Resolvers;

namespace Built.Micro.ImageCloud
{
    /*
     docker run -itd -p 27017:27017 --restart always mongo
     docker run -itd -p 8081:8081 --link mongodb:mongo  --restart always  mongo-express
     http://192.168.1.230:8081/
    */

    /// <summary>
    /// Magick.NET ImageResizer ImageSharp SixLabors.ImageSharp还不是正式版，nuget默认没有；SixLabors.ImageSharp.Web
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBuilt(options =>
            {
                options.UseMongodb("mongodb://192.168.1.230:27017/ImageCloud");
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddImageSharpCore()
               .SetRequestParser<QueryCollectionRequestParser>()
               .SetBufferManager<PooledBufferManager>()
               .SetCache(provider => new PhysicalFileSystemCache(
                   provider.GetRequiredService<IHostingEnvironment>(),
                   provider.GetRequiredService<IBufferManager>(),
                   provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>())
               {
                   Settings =
                   {
                        [PhysicalFileSystemCache.Folder] = PhysicalFileSystemCache.DefaultCacheFolder,
                        [PhysicalFileSystemCache.CheckSourceChanged] = "true"
                   }
               })
               .SetCacheHash<CacheHash>()
               .SetAsyncKeyLock<AsyncKeyLock>()
               .AddResolver<PhysicalFileSystemResolver>()
               .AddProcessor<ResizeWebProcessor>()
               .AddProcessor<FormatWebProcessor>()
               .AddProcessor<BackgroundColorWebProcessor>();

            //// Add the default service and options.
            //services.AddImageSharp();

            //// Or add the default service and custom options.
            //services.AddImageSharp(
            //    options =>
            //        {
            //            options.Configuration = Configuration.Default;
            //            options.MaxBrowserCacheDays = 7;
            //            options.MaxCacheDays = 365;
            //            options.CachedNameLength = 8;
            //            options.OnValidate = _ => { };
            //            options.OnBeforeSave = _ => { };
            //            options.OnProcessed = _ => { };
            //            options.OnPrepareResponse = _ => { };
            //        });

            //// Or we can fine-grain control adding the default options and configure all other services.
            //services.AddImageSharpCore()
            //        .SetRequestParser<QueryCollectionRequestParser>()
            //        .SetBufferManager<PooledBufferManager>()
            //        .SetCache<PhysicalFileSystemCache>()
            //        .SetCacheHash<CacheHash>()
            //        .SetAsyncKeyLock<AsyncKeyLock>()
            //        .AddResolver<PhysicalFileSystemResolver>()
            //        .AddProcessor<ResizeWebProcessor>()
            //        .AddProcessor<FormatWebProcessor>()
            //        .AddProcessor<BackgroundColorWebProcessor>();

            //// Or we can fine-grain control adding custom options and configure all other services
            //// There are also factory methods for each builder that will allow building from configuration files.
            //services.AddImageSharpCore(
            //    options =>
            //        {
            //            options.Configuration = Configuration.Default;
            //            options.MaxBrowserCacheDays = 7;
            //            options.MaxCacheDays = 365;
            //            options.CachedNameLength = 8;
            //            options.OnValidate = _ => { };
            //            options.OnBeforeSave = _ => { };
            //            options.OnProcessed = _ => { };
            //            options.OnPrepareResponse = _ => { };
            //        })
            //    .SetRequestParser<QueryCollectionRequestParser>()
            //    .SetBufferManager<PooledBufferManager>()
            //    .SetCache(provider =>
            //      {
            //          var p = new PhysicalFileSystemCache(
            //              provider.GetRequiredService<IHostingEnvironment>(),
            //              provider.GetRequiredService<IBufferManager>(),
            //              provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>());

            //          p.Settings[PhysicalFileSystemCache.Folder] = PhysicalFileSystemCache.DefaultCacheFolder;
            //          p.Settings[PhysicalFileSystemCache.CheckSourceChanged] = "true";

            //          return p;
            //      })
            //    .SetCacheHash<CacheHash>()
            //    .SetAsyncKeyLock<AsyncKeyLock>()
            //    .AddResolver<PhysicalFileSystemResolver>()
            //    .AddProcessor<ResizeWebProcessor>()
            //    .AddProcessor<FormatWebProcessor>()
            //    .AddProcessor<BackgroundColorWebProcessor>();

            // 注册自定义用户类
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped(typeof(MaterialRepository));
            services.AddScoped(typeof(IMaterialService), typeof(MaterialService));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseImageSharp();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseDefaultFiles();
            app.UseMvc();
        }
    }
}