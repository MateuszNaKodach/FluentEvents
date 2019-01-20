using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FluentEvents.Transmission
{
    public class EventReceiversHostedService : IHostedService
    {
        private readonly IEventReceiversService m_EventReceiversService;

        public EventReceiversHostedService(IEventReceiversService eventReceiversService)
        {
            m_EventReceiversService = eventReceiversService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return m_EventReceiversService.StartReceiversAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return m_EventReceiversService.StopReceiversAsync(cancellationToken);
        }
    }
}