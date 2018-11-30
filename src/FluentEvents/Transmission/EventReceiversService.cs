using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Transmission
{
    public class EventReceiversService : IEventReceiversService
    {
        private readonly ILogger<EventReceiversService> m_Logger;
        private readonly IEnumerable<IEventReceiver> m_EventReceivers;

        public EventReceiversService(ILogger<EventReceiversService> logger, IEnumerable<IEventReceiver> eventReceivers)
        {
            m_Logger = logger;
            m_EventReceivers = eventReceivers;
        }

        public async Task StartReceiversAsync(CancellationToken cancellationToken = default)
        {
            foreach (var eventReceiver in m_EventReceivers)
            {
                m_Logger.EventReceiverStarting(eventReceiver.GetType().Name);

                await eventReceiver.StartReceivingAsync(cancellationToken);

                m_Logger.EventReceiverStarted(eventReceiver.GetType().Name);
            }
        }

        public async Task StopReceiversAsync(CancellationToken cancellationToken = default)
        {
            foreach (var eventReceiver in m_EventReceivers)
            {
                m_Logger.EventReceiverStopping(eventReceiver.GetType().Name);

                await eventReceiver.StopReceivingAsync(cancellationToken);
                
                m_Logger.EventReceiverStopped(eventReceiver.GetType().Name);
            }
        }
    }
}