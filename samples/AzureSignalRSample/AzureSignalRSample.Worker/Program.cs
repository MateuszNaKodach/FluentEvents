using System.Threading.Tasks;
using AzureSignalRSample.Domain;
using AzureSignalRSample.Events;
using AzureSignalRSample.Persistence;
using AzureSignalRSample.Repositories;
using FluentEvents;
using FluentEvents.Azure.SignalR;
using FluentEvents.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureSignalRSample.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true)
                        .AddUserSecrets(typeof(Program).Assembly);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddEventsContext<AppEventsContext>(options =>
                    {
                        options.AttachToDbContextEntities<AppDbContext>();
                        options.UseAzureSignalRService(hostContext.Configuration.GetSection("Azure:SignalR"));
                    });

                    services.AddWithEventsAttachedTo<AppEventsContext>(() =>
                    {
                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseSqlServer(hostContext.Configuration.GetSection("Database:ConnectionString").Value);
                        });
                    });

                    services.AddScoped<ILightBulbRepository, LightBulbRepository>();
                    services.AddScoped<ILightBulbTogglingService, LightBulbTogglingService>();
                    services.AddHostedService<LightBulbTogglingHostedService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
