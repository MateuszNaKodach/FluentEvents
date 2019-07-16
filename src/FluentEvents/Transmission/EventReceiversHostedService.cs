using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FluentEvents.Transmission
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class EventReceiversHostedService : IHostedService
    {
        private readonly IEventReceiversService _eventReceiversService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public EventReceiversHostedService(IEventsContext eventsContext)
        {
            _eventReceiversService = eventsContext.Get<IServiceProvider>().GetRequiredService<IEventReceiversService>();
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _eventReceiversService.StartReceiversAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _eventReceiversService.StopReceiversAsync(cancellationToken);
        }
    }
}