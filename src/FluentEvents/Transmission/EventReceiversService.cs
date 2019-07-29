using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Transmission
{
    internal class EventReceiversService : IEventReceiversService
    {
        private readonly ILogger<EventReceiversService> _logger;
        private readonly IEnumerable<IEventReceiver> _eventReceivers;

        public EventReceiversService(ILogger<EventReceiversService> logger, IEnumerable<IEventReceiver> eventReceivers)
        {
            _logger = logger;
            _eventReceivers = eventReceivers;
        }

        public async Task StartReceiversAsync(CancellationToken cancellationToken = default)
        {
            foreach (var eventReceiver in _eventReceivers)
            {
                _logger.EventReceiverStarting(eventReceiver.GetType().Name);

                await eventReceiver.StartReceivingAsync(cancellationToken).ConfigureAwait(false);

                _logger.EventReceiverStarted(eventReceiver.GetType().Name);
            }
        }

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