using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FluentEvents
{
    /// <summary>
    ///     An interface for <see cref="EventsContext"/>s.
    /// </summary>
    public interface IEventsContext
    {
        /// <summary>
        ///     Attach an event source to the context in order to forward it's events to the configured pipelines.
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="eventsScope">The scope of the events published from this source.</param>
        void Attach(object source, EventsScope eventsScope);

        /// <summary>
        ///     Continues the processing of the events that have been queued.
        /// </summary>
        /// <param name="eventsScope">The scope where the queued events were published.</param>
        /// <param name="queueName">
        ///     The name of the queue.
        ///     If null all the events will be processed.
        /// </param>
        Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName = null);

        /// <summary>
        ///     Discards all the events of a queue.
        /// </summary>
        /// <param name="eventsScope">The scope where the queued events were published.</param>
        /// <param name="queueName">
        ///     The name of the queue.
        ///     If null all the events will be discarded.
        /// </param>
        void DiscardQueuedEvents(EventsScope eventsScope, string queueName = null);

        /// <summary>
        ///     Returns an <see cref="IHostedService"/> that can start or stop the configured event receivers.
        /// </summary>
        IHostedService GetEventReceiversHostedService();
    }
}