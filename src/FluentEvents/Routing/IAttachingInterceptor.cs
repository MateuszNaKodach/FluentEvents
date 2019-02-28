namespace FluentEvents.Routing
{
    /// <summary>
    ///     This interface can be implemented to intercept every attaching made to the <see cref="EventsContext"/>.
    /// </summary>
    public interface IAttachingInterceptor
    {
        /// <summary>
        ///     This method can be used to handle complex attaching situations (For example attaching to the entities of an ORM).
        /// </summary>
        /// <param name="attachingService">The attaching service that invokes the interceptor.</param>
        /// <param name="source">The events source being attached.</param>
        /// <param name="eventsScope">The current <see cref="EventsScope"/></param>
        void OnAttaching(IAttachingService attachingService, object source, EventsScope eventsScope);
    }
}