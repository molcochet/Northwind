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
        }
    }

}
