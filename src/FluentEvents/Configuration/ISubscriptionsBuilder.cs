using System;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Allows subscriptions configuration to be performed for a service type and an event type.
    /// </summary>
    public interface ISubscriptionsBuilder
    {
        /// <summary>
        ///     Allows configuration for required event subscriptions, to be performed for a service type.
        ///     (Multiple services with the same type are supported)
        ///     When a configured subscription receives an event and the <see cref="IServiceProvider"/>
        ///     doesn't return any service of this type an exception is thrown during publishing.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <returns>
        ///     Returns an object that can be used to configure subscriptions for the service type.
        /// </returns>
        ServiceHandlerConfiguration<TService, TEvent> ServiceHandler<TService, TEvent>()
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class;

        /// <summary>
        ///     Allows configuration for optional event subscriptions, to be performed for a service type.
        ///     (Multiple services with the same type are supported)
        ///     When a configured subscription receives an event and the <see cref="IServiceProvider"/>
        ///     doesn't return any service of this type no exceptions will be thrown.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <returns>
        ///     Returns an object that can be used to configure subscriptions for the service type.
        /// </returns>
        ServiceHandlerConfiguration<TService, TEvent> OptionalServiceHandler<TService, TEvent>()
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class;
    }
}