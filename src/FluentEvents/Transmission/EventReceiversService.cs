using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Transmission
{
    /// <inheritdoc />
    internal class EventReceiversService : IEventReceiversService
    {
        private readonly ILogger<EventReceiversService> _logger;
        private readonly IEnumerable<IEventReceiver> _eventReceivers;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public EventReceiversService(ILogger<EventReceiversService> logger, IEnumerable<IEventReceiver> eventReceivers)
        {
            _logger = logger;
            _eventReceivers = eventReceivers;
        }

        /// <inheritdoc />
        public async Task StartReceiversAsync(CancellationToken cancellationToken = default)
        {
            foreach (var eventReceiver in _eventReceivers)
            {
                _logger.EventReceiverStarting(eventReceiver.GetType().Name);

                await eventReceiver.StartReceivingAsync(cancellationToken).ConfigureAwait(false);

                _logger.EventReceiverStarted(eventReceiver.GetType().Name);
            }
        }

        /// <inheritdoc />
        public async Task StopReceiversAsync(CancellationToken cancellationToken = default)
        {
            foreach (var eventReceiver in _eventReceivers)
            {
                _logger.EventReceiverStopping(eventReceiver.GetType().Name);

                await eventReceiver.StopReceivingAsync(cancellationToken).ConfigureAwait(false);
                
                _logger.EventReceiverStopped(eventReceiver.GetType().Name);
            }
        }
    }
}