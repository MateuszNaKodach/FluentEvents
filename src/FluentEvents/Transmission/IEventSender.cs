using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Transmission
{
    /// <summary>
    ///     Represents a service that send events to an <see cref="IEventReceiver"/>.
    /// </summary>
    public interface IEventSender
    {
        /// <summary>
        ///     This method should send the event to an <see cref="IEventReceiver"/>.
        /// </summary>
        /// <param name="pipelineEvent">The event to send.</param>
        Task SendAsync(PipelineEvent pipelineEvent);
    }
}