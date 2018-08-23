using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace Built.Mongo
{
    public static class ServiceCollectionExtensions
    {
        public static BuiltBuilder AddBuilt(this IServiceCollection services, Action<BuiltOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }
            services.Configure(setupAction);
            /*
             services.Configure<BuiltOptions>(options =>
            {
                options.UseMongodb("");
            });
             */
            var options = new BuiltOptions();
            setupAction(options);
            services.AddSingleton(options);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            return new BuiltBuilder(services);
        }
    }

    public sealed class BuiltBuilder
    {
        public BuiltBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

    public static class BuiltOptionsExtensions
    {
        public static BuiltOptions UseMongodb(this BuiltOptions options, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            options.ConnectionString = connectionString;
            return options.UseMongodb(new MongoUrl(connectionString));
        }

        public static BuiltOptions UseMongodb(this BuiltOptions options, MongoClientSettings configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            options.Settings = configure;
            return options;
        }

        public static BuiltOptions UseMongodb(this BuiltOptions options, MongoUrl url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            options.Url = url;
            return options.UseMongodb(MongoClientSettings.FromUrl(url));
        }
    }

    public class BuiltOptions : IOptions<BuiltOptions>
    {
        public MongoClientSettings Settings { get; set; }
        public MongoUrl Url { get; set; }

        public string ConnectionString { get; set; }

        BuiltOptions IOptions<BuiltOptions>.Value
        {
            get
            {
                return this;
            }
        }
    }
}