using System;
using System.Threading;
using System.Threading.Tasks;
using AzureSignalRSample.DomainModel;
using AzureSignalRSample.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureSignalRSample.Worker
{
    public class LightBulbTogglingHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LightBulbTogglingHostedService> _logger;
        private Timer _timer;

        public LightBulbTogglingHostedService(
            IServiceProvider serviceProvider,
            ILogger<LightBulbTogglingHostedService> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ => await DoWork(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            _logger.LogInformation("Toggling light bulb");

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var lightBulbTogglingService = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ILightBulbTogglingService>();

                var appDbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<AppDbContext>();

                await lightBulbTogglingService.ToggleLightBulbAsync("Toggled by worker");

                await appDbContext.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}