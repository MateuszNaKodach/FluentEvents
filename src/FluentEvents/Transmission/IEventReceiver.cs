using System.Threading;
using System.Threading.Tasks;

namespace FluentEvents.Transmission
{
    /// <summary>
    ///     Represents a service that receives events transmitted from an <see cref="IEventSender"/>.
    /// </summary>
    public interface IEventReceiver
    {
        /// <summary>
        ///     This method should start a worker that receives events transmitted from an <see cref="IEventSender"/>. 
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start receiving process has been aborted.</param>
        Task StartReceivingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     This method should stop the worker that receives events transmitted from an <see cref="IEventSender"/>. 
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start receiving process has been aborted.</param>
        Task StopReceivingAsync(CancellationToken cancellationToken = default);
    }
}
