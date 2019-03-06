namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureTopicEventReceiverConfig.SubscriptionNameGenerator" /> property is null.
    /// </summary>
    public class SubscriptionNameGeneratorIsNullException : FluentEventsServiceBusException
    {
    }
}