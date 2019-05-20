using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkerSample.DomainModel;
using WorkerSample.Repositories;

namespace WorkerSample.Worker
{
    public class ProcessExpiredSubscriptionsHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public ProcessExpiredSubscriptionsHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ => await DoWork(), null, TimeSpan.FromSeconds(2), TimeSpan.Zero);

            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var productSubscriptionCancellationService = serviceScope
                    .ServiceProvider
                    .GetRequiredService<IProductSubscriptionCancellationService>();

                var appDbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<AppDbContext>();

                productSubscriptionCancellationService.CancelExpiredSubscriptions(DateTime.UtcNow);

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
