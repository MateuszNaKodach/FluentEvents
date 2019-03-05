namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="TopicEventReceiverConfig.SubscriptionNameGenerator" /> property is null.
    /// </summary>
    public class SubscriptionNameGeneratorIsNullException : FluentEventsServiceBusException
    {
    }
}