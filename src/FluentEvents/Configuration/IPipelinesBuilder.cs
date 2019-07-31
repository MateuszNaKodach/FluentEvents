namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Allows pipelines configuration to be performed for event types.
    /// </summary>
    public interface IPipelinesBuilder
    {
        /// <summary>
        ///     Allows configuration to be performed for an event type.
        ///     This method can be called multiple times for the same event
        ///     type in order to configure multiple pipelines.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns>The configuration object for the specified event.</returns>
        EventConfiguration<TEvent> Event<TEvent>()
            where TEvent : class;
    }
}