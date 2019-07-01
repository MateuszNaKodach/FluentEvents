using System;
using System.Threading.Tasks;
using FluentEvents;
using FluentEvents.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerSample.Application;
using WorkerSample.Domain;
using WorkerSample.Events;
using WorkerSample.Repositories;

namespace WorkerSample.Worker
{
    public static class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
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
                    
                    services.AddScoped<IProductSubscriptionsRepository, ProductSubscriptionsRepository>();
                    services.AddScoped<IProductSubscriptionCancellationService, ProductSubscriptionCancellationService>();
                    services.AddScoped<IMailService, FakeMailService>();
                    services.AddScoped<ProductSubscriptionCancelledEventHandler>();
                    services.AddHostedService<ProcessExpiredSubscriptionsHostedService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.SetMinimumLevel(LogLevel.Warning);
                    configLogging.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var appDbContext = serviceScope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                appDbContext.ProductSubscriptions.Add(
                    new ProductSubscription(1, "customer@email", DateTime.MinValue)
                );

                await appDbContext.SaveChangesAsync();
            }

            await host.RunAsync();
        }
    }
}
