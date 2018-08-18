using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using static IdentityServer4.IdentityServerConstants;

namespace Built.Micro.OAuthCloud
{
    //http://docs.identityserver.io/en/release/quickstarts/6_aspnet_identity.html
    /// <summary>
    /// DotNetOpenAuth(有灰) IdentityServer4
    /// (OpenID/OAuth/OAuth2)
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // some details omitted
            services.AddIdentityServer()
                .AddInMemoryClients(new List<Client>
            {
               new Client{
                   //client_id
                    ClientId = "pwd_client",
                    AllowedGrantTypes = new string[] { GrantType.ResourceOwnerPassword },
                    //client_secret
                    ClientSecrets =
                    {
                        new Secret("pwd_secret".Sha256())
                    },
                    //scope
                    AllowedScopes =
                    {
                        "api1",
                        //如果想带有RefreshToken，那么必须设置：StandardScopes.OfflineAccess
                        StandardScopes.OfflineAccess,
                    },
                    //AccessTokenLifetime = 3600, //AccessToken的过期时间， in seconds (defaults to 3600 seconds / 1 hour)
                    //AbsoluteRefreshTokenLifetime = 60, //RefreshToken的最大过期时间，就算你使用了TokenUsage.OneTimeOnly模式，更新的RefreshToken最大期限也是为这个属性设置的(就是6月30日就得要过期[根据服务器时间]，你用旧的RefreshToken重新获取了新RefreshToken，新RefreshToken过期时间也是6月30日)， in seconds. Defaults to 2592000 seconds / 30 day
                    //RefreshTokenUsage = TokenUsage.OneTimeOnly,   //默认状态，RefreshToken只能使用一次，使用一次之后旧的就不能使用了，只能使用新的RefreshToken
                    //RefreshTokenUsage = TokenUsage.ReUse,   //重复使用RefreshToken，RefreshToken过期了就不能使用了
               }
            }).AddInMemoryIdentityResources(new List<IdentityResource> {
                new IdentityResource{ }
            });

            services.AddAuthentication()
              .AddGoogle("Google", options =>
              {
                  options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                  options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
                  options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";
              })

              .AddOpenIdConnect("demoidsrv", "IdentityServer", options =>
              {
                  options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                  options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                  options.Authority = "https://demo.identityserver.io/";
                  options.ClientId = "implicit";
                  options.ResponseType = "id_token";
                  options.SaveTokens = true;
                  options.CallbackPath = new PathString("/signin-idsrv");
                  options.SignedOutCallbackPath = new PathString("/signout-callback-idsrv");
                  options.RemoteSignOutPath = new PathString("/signout-idsrv");

                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      NameClaimType = "name",
                      RoleClaimType = "role"
                  };
              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}