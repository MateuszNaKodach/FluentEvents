namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureServiceBusTopicEventReceiverConfig.TopicPath" /> property is null.
    /// </summary>
    public class TopicPathIsNullException : FluentEventsServiceBusException
    {
        internal TopicPathIsNullException()
            : base("The topic path is null")
        {
            
        }
    }
}