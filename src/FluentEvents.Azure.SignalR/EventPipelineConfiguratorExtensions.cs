using System;
using FluentEvents.Infrastructure;
using FluentEvents.Config;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     Extensions for <see cref="EventPipelineConfigurator{TSource,TEventArgs}"/>
    /// </summary>
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the users connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <param name="hubName">The SignalR hub name.</param>
        /// <param name="hubMethodName">The SignalR hub method name.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToAllAzureSignalRUsers<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            string hubName,
            string hubMethodName
        )
            where TSource : class
            where TEventArgs : class
        {
            AddModule(eventPipelineConfigurator, PublicationMethod.Broadcast, hubName, hubMethodName, null);

            return eventPipelineConfigurator;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to some users connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <param name="hubName">The SignalR hub name.</param>
        /// <param name="hubMethodName">The SignalR hub method name.</param>
        /// <param name="userIdsProviderAction">
        ///     A <see cref="Func{TResult}"/> that returns the ids of the users that should receive the event.
        /// </param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToAzureSignalRUsers<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            string hubName,
            string hubMethodName,
            Func<TSource, TEventArgs, string[]> userIdsProviderAction
        )
            where TSource : class
            where TEventArgs : class
        {
            AddModule(eventPipelineConfigurator, PublicationMethod.User, hubName, hubMethodName, userIdsProviderAction);

            return eventPipelineConfigurator;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to some groups connected to
        ///     the configured Azure SignalR Service.
        /// </summary>
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <param name="hubName">The SignalR hub name.</param>
        /// <param name="hubMethodName">The SignalR hub method name.</param>
        /// <param name="groupIdsProviderAction">
        ///     A <see cref="Func{TResult}"/> that returns the ids of the groups that should receive the event.
        /// </param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToAzureSignalRGroups<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            string hubName,
            string hubMethodName,
            Func<TSource, TEventArgs, string[]> groupIdsProviderAction
        )
            where TSource : class
            where TEventArgs : class
        {
            AddModule(eventPipelineConfigurator, PublicationMethod.Group, hubName, hubMethodName, groupIdsProviderAction);

            return eventPipelineConfigurator;
        }

        private static void AddModule<TSource, TEventArgs>(
            EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            Func<TSource, TEventArgs, string[]> receiverIdsProviderAction
        )
            where TSource : class 
            where TEventArgs : class
        {
            ((IInfrastructure<IPipeline>) eventPipelineConfigurator).Instance
                .AddModule<AzureSignalRPipelineModule, AzureSignalRPipelineModuleConfig>(
                    new AzureSignalRPipelineModuleConfig
                    {
                        PublicationMethod = publicationMethod,
                        HubMethodName = hubMethodName,
                        HubName = hubName,
                        ReceiverIdsProviderAction = (Func<object, object, string[]>) receiverIdsProviderAction
                    }
                );
        }
    }
}
