using System;
using FluentEvents.Infrastructure;
using FluentEvents.Configuration;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     Extensions for <see cref="EventPipelineConfiguration{TEvent}"/>
    /// </summary>
    public static class EventPipelineConfigurationExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the users connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="hubName">The SignalR hub name. The default value is the entity name with the "Hub" suffix (Eg. TSourceHub).</param>
        /// <param name="hubMethodName">The SignalR hub method name. The default value is the current event field name.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfiguration<TEvent> ThenIsSentToAllAzureSignalRUsers<TEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration,
            string hubName,
            string hubMethodName = null
        )
            where TEvent : class
        {
            if (eventPipelineConfiguration == null) throw new ArgumentNullException(nameof(eventPipelineConfiguration));
            if (hubName == null) throw new ArgumentNullException(nameof(hubName));

            AddModule(eventPipelineConfiguration, PublicationMethod.Broadcast, hubName, hubMethodName, null);

            return eventPipelineConfiguration;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to some users connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="userIdsProviderAction">
        ///     A <see cref="Func{TResult}"/> that returns the ids of the users that should receive the event.
        /// </param>
        /// <param name="hubName">The SignalR hub name. The default value is the entity name with the "Hub" suffix (Eg. TSourceHub).</param>
        /// <param name="hubMethodName">The SignalR hub method name. The default value is the current event field name.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfiguration<TEvent> ThenIsSentToAzureSignalRUsers<TEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration,
            Func<TEvent, string[]> userIdsProviderAction,
            string hubName,
            string hubMethodName = null
        )
            where TEvent : class
        {
            if (eventPipelineConfiguration == null) throw new ArgumentNullException(nameof(eventPipelineConfiguration));
            if (hubName == null) throw new ArgumentNullException(nameof(hubName));

            AddModule(eventPipelineConfiguration, PublicationMethod.User, hubName, hubMethodName, userIdsProviderAction);

            return eventPipelineConfiguration;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to some groups connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="groupIdsProviderAction">
        ///     A <see cref="Func{TResult}"/> that returns the ids of the groups that should receive the event.
        /// </param>
        /// <param name="hubName">The SignalR hub name. The default value is the entity name with the "Hub" suffix (Eg. TSourceHub).</param>
        /// <param name="hubMethodName">The SignalR hub method name. The default value is the current event field name.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfiguration<TEvent> ThenIsSentToAzureSignalRGroups<TEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration,
            Func<TEvent, string[]> groupIdsProviderAction,
            string hubName,
            string hubMethodName = null
        )
            where TEvent : class
        {
            if (eventPipelineConfiguration == null) throw new ArgumentNullException(nameof(eventPipelineConfiguration));
            if (hubName == null) throw new ArgumentNullException(nameof(hubName));

            AddModule(eventPipelineConfiguration, PublicationMethod.Group, hubName, hubMethodName, groupIdsProviderAction);

            return eventPipelineConfiguration;
        }

        private static void AddModule<TEvent>(
            EventPipelineConfiguration<TEvent> eventPipelineConfiguration,
            PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            Func<TEvent, string[]> receiverIdsProviderAction
        )
            where TEvent : class
        {
            if (hubMethodName == null)
                hubMethodName = typeof(TEvent).Name;

            ((IInfrastructure<IPipeline>) eventPipelineConfiguration).Instance
                .AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(
                    new AzureSignalRPipelineModuleConfig
                    {
                        PublicationMethod = publicationMethod,
                        HubMethodName = hubMethodName,
                        HubName = hubName,
                        ReceiverIdsProviderAction = receiverIdsProviderAction == null 
                            ? (Func<object, string[]>)null 
                            : e => receiverIdsProviderAction((TEvent) e)
                    }
                );
        }
    }
}
