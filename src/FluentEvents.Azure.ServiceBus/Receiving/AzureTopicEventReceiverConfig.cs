using System;
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events receiver.
    /// </summary>
    public class AzureTopicEventReceiverConfig : IValidableConfig
    {
        private string _managementConnectionString;
        private string _receiveConnectionString;
        private Func<string> _subscriptionNameGenerator = () => Guid.NewGuid().ToString();

        /// <summary>
        ///     Path of the Azure Service Bus topic relative to the namespace base address.
        /// </summary>
        public string TopicPath { get; set; }

        /// <summary>
        ///     A connection string that can be used to dynamically create topic subscriptions.
        /// </summary>
        public string ManagementConnectionString
        {
            get => _managementConnectionString;
            set => _managementConnectionString = ConnectionStringValidator.ValidateOrThrow(value);
        }

        /// <summary>
        ///     A connection string that can be used to receive messages from a topic subscription.
        /// </summary>
        public string ReceiveConnectionString
        {
            get => _receiveConnectionString;
            set => _receiveConnectionString = ConnectionStringValidator.ValidateOrThrow(value);
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
        /// <remarks>The default implementation returns a GUID.</remarks>
        public Func<string> SubscriptionNameGenerator
        {
            get => _subscriptionNameGenerator;
            set => _subscriptionNameGenerator = value ?? throw new ArgumentNullException(nameof(value));
        }

        void IValidableConfig.Validate()
        {
            if (ReceiveConnectionString == null)
                throw new ReceiveConnectionStringIsNullException();
            if (ManagementConnectionString == null)
                throw new ManagementConnectionStringIsNullException();
            if (TopicPath == null)
                throw new TopicPathIsNullException();
        }
    }
}