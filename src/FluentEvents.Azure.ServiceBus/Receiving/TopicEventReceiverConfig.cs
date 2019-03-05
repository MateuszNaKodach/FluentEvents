using System;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events receiver.
    /// </summary>
    public class TopicEventReceiverConfig
    {
        private string m_ManagementConnectionString;
        private string m_ReceiveConnectionString;

        /// <summary>
        ///     Path of the Azure Service Bus topic relative to the namespace base address.
        /// </summary>
        public string TopicPath { get; set; }

        /// <summary>
        ///     A connection string that can be used to dynamically create topic subscriptions.
        /// </summary>
        public string ManagementConnectionString
        {
            get => m_ManagementConnectionString;
            set => m_ManagementConnectionString = ConnectionStringValidator.ValidateOrThrow(value);
        }

        /// <summary>
        ///     A connection string that can be used to receive messages from a topic subscription.
        /// </summary>
        public string ReceiveConnectionString
        {
            get => m_ReceiveConnectionString;
            set => m_ReceiveConnectionString = ConnectionStringValidator.ValidateOrThrow(value);
        }

        /// <summary>
        ///     The <see cref="TimeSpan"/> idle interval after which the subscription is automatically deleted.
        /// </summary>
        /// <remarks>The minimum duration is 5 minutes. Default value is <see cref="TimeSpan.MaxValue"/>.</remarks>
        public TimeSpan SubscriptionsAutoDeleteOnIdleTimeout { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        ///     Gets or sets the maximum number of concurrent calls to the callback the message pump should initiate.
        /// </summary>
        /// <remarks>The default value is 1.</remarks>
        public int MaxConcurrentMessages { get; set; } = 1;

        /// <summary>
        ///     A <see cref="Func{TResult}" /> that returns unique names for subscriptions.
        /// </summary>
        /// <remarks>The default implementations returns a GUID.</remarks>
        public Func<string> SubscriptionNameGenerator { get; set; } = () => Guid.NewGuid().ToString();
    }
}