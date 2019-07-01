using AspNetCoreApiSample.Domain;
using AspNetCoreApiSample.Events;
using AspNetCoreApiSample.Notifications;
using AspNetCoreApiSample.Repositories;
using FluentEvents;
using FluentEvents.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCoreApiSample.Api
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "AspNetCoreApiSample API", Version = "v1" });
            });

            services.AddEventsContext<AppEventsContext>(options =>
            {
                options.AttachToDbContextEntities<AppDbContext>();
            });

            services.AddWithEventsAttachedTo<AppEventsContext>(() =>
            {
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("SampleDatabase");
                });
            });

            services.AddScoped<IContractsRepository, ContractsRepository>();
            services.AddScoped<IContractTerminationService, ContractTerminationService>();
            services.AddSingleton<IMailService, MailService>();
            services.AddHostedService<NotificationsHostedService>();
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
                app.UseHsts();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCoreApiSample V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
