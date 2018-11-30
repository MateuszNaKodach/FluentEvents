using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FluentEvents.Transmission
{
    public class EventReceiversHostedService : IHostedService
    {
        private readonly EventsContext m_EventsContext;

        public EventReceiversHostedService(EventsContext eventsContext)
        {
            m_EventsContext = eventsContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return m_EventsContext.StartEventReceivers(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return m_EventsContext.StopEventReceivers(cancellationToken);
        }
    }
}