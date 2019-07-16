using System.Collections.Generic;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;

namespace FluentEvents.Infrastructure
{
    /// <summary>
    ///     The <see cref="IEventsScope"/> represents the scope where entities are attached and the events
    ///     are handled or queued.
    /// </summary>
    public interface IEventsScope
    {
        /// <summary>
        ///     Gets or creates a new queue with the given name
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <returns></returns>
        IEventsQueue GetOrAddEventsQueue(string queueName);

        /// <summary>
        ///     Gets or creates all the scoped subscriptions.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Subscription}"/> of subscriptions.</returns>
        IEnumerable<Subscription> GetSubscriptions();

        /// <summary>
        ///     Gets all the event queues created.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{IEventsQueue}"/> of event queues.</returns>
        IEnumerable<IEventsQueue> GetEventsQueues();
    }
}