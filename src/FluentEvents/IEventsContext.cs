using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System;

namespace FluentEvents
{
    /// <summary>
    ///     An interface for <see cref="EventsContext"/>s.
    /// </summary>
    public interface IEventsContext
    {
        /// <summary>
        ///     Subscribes the <see cref="EventsContext"/> to every source's event field having a delegate with
        ///     a single parameter (of any type) and a return type of <see cref="Task"/> or <see langword="void"/>.
        ///
        ///     Examples:
        ///     <see cref="Func{TEvent, TTask}">Func&lt;<see cref="object"/>, <see cref="Task"/>&gt;</see> or
        ///     <see cref="Action{TEvent}" /> or
        ///     <see cref="AsyncEventPublisher{TEvent}" /> or
        ///     <see cref="EventPublisher{TEvent}" />
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="eventsScope">The scope of the events published from this source.</param>
        void WatchSourceEvents(object source, EventsScope eventsScope);

        /// <summary>
        ///     Resumes the processing of the events that have been queued.
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