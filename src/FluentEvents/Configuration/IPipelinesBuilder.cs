namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Provides a simple API surface to select an event and configure it fluently.
    /// </summary>
    public interface IPipelinesBuilder
    {
        /// <summary>
        ///     Returns an object that can be used to configure fluently how the event is handled.
        ///     This method can be called multiple times for the same event to
        ///     configure multiple pipelines.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns>The configuration object for the specified event.</returns>
        EventConfiguration<TEvent> Event<TEvent>()
            where TEvent : class;
    }
}