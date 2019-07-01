using AzureSignalRSample.Domain;
using AzureSignalRSample.Events;
using AzureSignalRSample.Persistence;
using AzureSignalRSample.Query;
using AzureSignalRSample.Repositories;
using AzureSignalRSample.Web.Hubs;
using FluentEvents;
using FluentEvents.Azure.SignalR;
using FluentEvents.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureSignalRSample.Web
{
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR().AddAzureSignalR();

            services.AddEventsContext<AppEventsContext>(options =>
            {
                options.AttachToDbContextEntities<AppDbContext>();
                options.UseAzureSignalRService(Configuration.GetSection("Azure:SignalR"));
            });

            services.AddWithEventsAttachedTo<AppEventsContext>(() =>
            {
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetSection("Database:ConnectionString").Value);
                });
            });

            services.AddScoped<ILightBulbRepository, LightBulbRepository>();
            services.AddScoped<ILightBulbTogglingService, LightBulbTogglingService>();
            services.AddScoped<ILightBulbQueryService, LightBulbQueryService>();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<LightBulbHub>("/lightBulb");
            });
        }
    }
}
