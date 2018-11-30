using System.Threading;
using System.Threading.Tasks;

namespace FluentEvents.Transmission
{
    public interface IEventReceiver
    {
        Task StartReceivingAsync(CancellationToken cancellationToken = default);
        Task StopReceivingAsync(CancellationToken cancellationToken = default);
    }
}
