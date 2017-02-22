using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Funq;
using ServiceStack;
using Northwind.ServiceInterface;
using ServiceStack.Admin;
using ServiceStack.OrmLite;
using ServiceStack.Data;
using ServiceStack.Auth;

namespace Northwind
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost());
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("Northwind Web Services", typeof(CustomersService).GetAssembly()) { }

        public override void Configure(Container container)
        {
            container.Register<IDbConnectionFactory>(
                new OrmLiteConnectionFactory(MapProjectPath("~/App_Data/Northwind.sqlite"), SqliteDialect.Provider));

            //Use Redis Cache
            //container.Register<ICacheClient>(new PooledRedisClientManager());

            VCardFormat.Register(this);

            Plugins.Add(new AutoQueryFeature { MaxLimit = 100 });
            Plugins.Add(new AdminFeature());

            Plugins.Add(new CorsFeature());
            //Add auth support
            Plugins.Add(new AuthFeature(() => new CustomUserSession(),
                new IAuthProvider[]
                {
                    new CredentialsAuthProvider(),              //HTML Form post of UserName/Password credentials
                    new BasicAuthProvider(),                    //Sign-in with HTTP Basic Auth
                    new DigestAuthProvider(AppSettings),        //Sign-in with HTTP Digest Auth
                    new TwitterAuthProvider(AppSettings),       //Sign-in with Twitter
                    new FacebookAuthProvider(AppSettings),      //Sign-in with Facebook
                    new GithubAuthProvider(AppSettings),        //Sign-in with GitHub OAuth Provider
                })
            {
                HtmlRedirect = "/",
                IncludeRegistrationService = true,
            });
            //Register the OrmLite repository to store the auth details as they are registered with the system
            container.Register<IAuthRepository>(c =>
            new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>())
            {
                UseDistinctRoleTables = AppSettings.Get("UseDistinctRoleTables", true),
            });
            var authRepo = (OrmLiteAuthRepository)container.Resolve<IAuthRepository>();
            SessionService.ResetUsers(authRepo);
        }
    }

}
