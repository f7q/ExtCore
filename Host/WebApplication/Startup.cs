using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WebApplication
{
    public class Startup : ExtCore.WebApplication.Startup
    {
        public Startup(IHostingEnvironment env) : base(env)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            this.configurationRoot = configurationBuilder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
            base.Configure(app, env);
        }
    }
}
