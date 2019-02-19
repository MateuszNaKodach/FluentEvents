using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FluentEvents.Transmission
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class EventReceiversHostedService : IHostedService
    {
        private readonly IEventReceiversService m_EventReceiversService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public EventReceiversHostedService(IEventReceiversService eventReceiversService)
        {
            m_EventReceiversService = eventReceiversService;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return m_EventReceiversService.StartReceiversAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return m_EventReceiversService.StopReceiversAsync(cancellationToken);
        }
    }
}