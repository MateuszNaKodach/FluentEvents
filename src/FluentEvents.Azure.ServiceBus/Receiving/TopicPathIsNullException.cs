namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureTopicEventReceiverOptions.TopicPath" /> property is null.
    /// </summary>
    public class TopicPathIsNullException : FluentEventsServiceBusException
    {
        internal TopicPathIsNullException()
            : base("The topic path is null")
        {
            
        }
    }
}