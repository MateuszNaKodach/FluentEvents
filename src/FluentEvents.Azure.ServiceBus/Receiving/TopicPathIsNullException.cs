namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureTopicEventReceiverConfig.TopicPath" /> property is null.
    /// </summary>
    public class TopicPathIsNullException : FluentEventsServiceBusException
    {
    }
}