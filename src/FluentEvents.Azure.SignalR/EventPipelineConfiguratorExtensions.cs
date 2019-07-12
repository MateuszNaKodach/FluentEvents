using System;
using FluentEvents.Infrastructure;
using FluentEvents.Config;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     Extensions for <see cref="EventPipelineConfigurator{TEvent}"/>
    /// </summary>
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the users connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="hubName">The SignalR hub name. The default value is the entity name with the "Hub" suffix (Eg. TSourceHub).</param>
        /// <param name="hubMethodName">The SignalR hub method name. The default value is the current event field name.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TEvent> ThenIsSentToAllAzureSignalRUsers<TEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator,
            string hubName,
            string hubMethodName = null
        )
            where TEvent : class
        {
            if (hubName == null) throw new ArgumentNullException(nameof(hubName));

            AddModule(eventPipelineConfigurator, PublicationMethod.Broadcast, hubName, hubMethodName, null);

            return eventPipelineConfigurator;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to some users connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="userIdsProviderAction">
        ///     A <see cref="Func{TResult}"/> that returns the ids of the users that should receive the event.
        /// </param>
        /// <param name="hubName">The SignalR hub name. The default value is the entity name with the "Hub" suffix (Eg. TSourceHub).</param>
        /// <param name="hubMethodName">The SignalR hub method name. The default value is the current event field name.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TEvent> ThenIsSentToAzureSignalRUsers<TEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator,
            Func<TEvent, string[]> userIdsProviderAction,
            string hubName,
            string hubMethodName = null
        )
            where TEvent : class
        {
            if (hubName == null) throw new ArgumentNullException(nameof(hubName));

            AddModule(eventPipelineConfigurator, PublicationMethod.User, hubName, hubMethodName, userIdsProviderAction);

            return eventPipelineConfigurator;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to some groups connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="groupIdsProviderAction">
        ///     A <see cref="Func{TResult}"/> that returns the ids of the groups that should receive the event.
        /// </param>
        /// <param name="hubName">The SignalR hub name. The default value is the entity name with the "Hub" suffix (Eg. TSourceHub).</param>
        /// <param name="hubMethodName">The SignalR hub method name. The default value is the current event field name.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TEvent> ThenIsSentToAzureSignalRGroups<TEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator,
            Func<TEvent, string[]> groupIdsProviderAction,
            string hubName,
            string hubMethodName = null
        )
            where TEvent : class
        {
            if (hubName == null) throw new ArgumentNullException(nameof(hubName));

            AddModule(eventPipelineConfigurator, PublicationMethod.Group, hubName, hubMethodName, groupIdsProviderAction);

            return eventPipelineConfigurator;
        }

        private static void AddModule<TEvent>(
            EventPipelineConfigurator<TEvent> eventPipelineConfigurator,
            PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            Func<TEvent, string[]> receiverIdsProviderAction
        )
            where TEvent : class
        {
            if (hubMethodName == null)
                hubMethodName = typeof(TEvent).Name;

            ((IInfrastructure<IPipeline>) eventPipelineConfigurator).Instance
                .AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(
                    new AzureSignalRPipelineModuleConfig
                    {
                        PublicationMethod = publicationMethod,
                        HubMethodName = hubMethodName,
                        HubName = hubName,
                        ReceiverIdsProviderAction = receiverIdsProviderAction == null 
                            ? (Func<object, string[]>)null 
                            : domainEvent => receiverIdsProviderAction((TEvent) domainEvent)
                    }
                );
        }
    }
}
