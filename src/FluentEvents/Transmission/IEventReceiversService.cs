using System.Threading;
using System.Threading.Tasks;

namespace FluentEvents.Transmission
{
    public interface IEventReceiversService
    {
        Task StartReceiversAsync(CancellationToken cancellationToken = default);
        Task StopReceiversAsync(CancellationToken cancellationToken = default);
    }
}
