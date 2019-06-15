using System;
using FluentEvents.Model;
using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides an API surface to configure the subscriptions of a service event handler.
    /// </summary>
    public class ServiceHandlerConfigurator<TService, TSource, TEventArgs>
        where TService : class, IEventHandler<TSource, TEventArgs>
        where TSource : class
        where TEventArgs : class
    {
        private readonly SourceModel _sourceModel;
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;
        private readonly IGlobalSubscriptionsService _globalSubscriptionsService;
        private readonly IEventSelectionService _eventSelectionService;

        internal ServiceHandlerConfigurator(
            SourceModel sourceModel,
            IScopedSubscriptionsService scopedSubscriptionsService, 
            IGlobalSubscriptionsService globalSubscriptionsService,
            IEventSelectionService eventSelectionService
        )
        {
            _sourceModel = sourceModel;
            _scopedSubscriptionsService = scopedSubscriptionsService;
            _globalSubscriptionsService = globalSubscriptionsService;
            _eventSelectionService = eventSelectionService;
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TSource,TEventArgs}.HandleEventAsync"/> to global events.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventArgsTypeMismatchException">
        ///     The specified event args type is different from the event args type of the event being selected.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TSource, TEventArgs> HasGlobalSubscription(string eventName)
        {
            CheckEventField(eventName);

            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TService, TSource, TEventArgs>(
                eventName
            );

            return this;
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TSource,TEventArgs}.HandleEventAsync"/> to global events.
        /// </summary>
        /// <param name="eventSelectionAction">
        ///     This parameter accepts an <see cref="Action{TSource, dynamic}"/> that subscribes the dynamic object
        ///     supplied in the second <see cref="Action{TSource, dynamic}"/> parameter to the event being selected.
        ///     Example usage: <code>(source, eventHandler) =&gt; source.MyEvent += eventHandler</code>
        /// </param>
        /// <example>
        ///     HasGlobalSubscription&lt;MySource, MyEventArgs&gt;((source, eventHandler) =&gt; source.MyEvent += eventHandler)
        /// </example>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventArgsTypeMismatchException">
        ///     The specified event args type is different from the event args type of the event being selected.
        /// </exception>
        /// <exception cref="MoreThanOneEventSelectedException">
        ///     More than one event selected.
        ///     The dynamic object provided in the selection action can only be subscribed once.
        /// </exception>
        /// <exception cref="NoEventsSelectedException">
        ///     The event selection action doesn't subscribe the provided dynamic object to any event.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TSource, TEventArgs> HasGlobalSubscription(
            Action<TSource, dynamic> eventSelectionAction
        )
        {
            var eventName = _eventSelectionService.GetSingleSelectedEventName(_sourceModel, eventSelectionAction);

            return HasGlobalSubscription(eventName);
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TSource,TEventArgs}.HandleEventAsync"/> to scoped events.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventArgsTypeMismatchException">
        ///     The specified event args type is different from the event args type of the event being selected.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TSource, TEventArgs> HasScopedSubscription(string eventName)
        {
            CheckEventField(eventName);

            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<TService, TSource, TEventArgs>(
                eventName
            );

            return this;
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TSource,TEventArgs}.HandleEventAsync"/> to global events.
        /// </summary>
        /// <param name="eventSelectionAction">
        ///     This parameter accepts an <see cref="Action{TSource, dynamic}"/> that subscribes the dynamic object
        ///     supplied in the second <see cref="Action{TSource, dynamic}"/> parameter to the event being selected.
        ///     Example usage: <code>(source, eventHandler) =&gt; source.MyEvent += eventHandler</code>
        /// </param>
        /// <example>
        ///     HasGlobalSubscription&lt;MySource, MyEventArgs&gt;((source, eventHandler) =&gt; source.MyEvent += eventHandler)
        /// </example>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventArgsTypeMismatchException">
        ///     The specified event args type is different from the event args type of the event being selected.
        /// </exception>
        /// <exception cref="MoreThanOneEventSelectedException">
        ///     More than one event selected.
        ///     The dynamic object provided in the selection action can only be subscribed once.
        /// </exception>
        /// <exception cref="NoEventsSelectedException">
        ///     The event selection action doesn't subscribe the provided dynamic object to any event.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TSource, TEventArgs> HasScopedSubscription(
            Action<TSource, dynamic> eventSelectionAction
        )
        {
            var eventName = _eventSelectionService.GetSingleSelectedEventName(_sourceModel, eventSelectionAction);

            return HasScopedSubscription(eventName);
        }

        private void CheckEventField(string eventName)
        {
            var eventField = _sourceModel.GetOrCreateEventField(eventName);

            if (eventField.EventArgsType != typeof(TEventArgs))
                throw new EventArgsTypeMismatchException();
        }
    }
}