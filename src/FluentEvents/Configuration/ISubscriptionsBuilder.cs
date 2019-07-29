using System;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Provides a simple API surface to select a service and configure it fluently.
    /// </summary>
    public interface ISubscriptionsBuilder
    {
        /// <summary>
        ///     Maps required services to the <see cref="EventsContext"/>.
        ///     If the <see cref="IServiceProvider"/> doesn't return any service of this type an exception is thrown during publishing.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <returns>
        ///     Returns an object that can be used to configure subscriptions for
        ///     an <see cref="IAsyncEventHandler{TEvent}.HandleEventAsync"/> method.
        /// </returns>
        ServiceHandlerConfiguration<TService, TEvent> ServiceHandler<TService, TEvent>()
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class;

        /// <summary>
        ///     Maps optional services to the <see cref="EventsContext"/>.
        ///     If the <see cref="IServiceProvider"/> doesn't return any service of this type no exceptions will be thrown during publishing.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <returns>
        ///     Returns an object that can be used to configure subscriptions for
        ///     an <see cref="IAsyncEventHandler{TEvent}.HandleEventAsync"/> method.
        /// </returns>
        ServiceHandlerConfiguration<TService, TEvent> OptionalServiceHandler<TService, TEvent>()
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class;
    }
}