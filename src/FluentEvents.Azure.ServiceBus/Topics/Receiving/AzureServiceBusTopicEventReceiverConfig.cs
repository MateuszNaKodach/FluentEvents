using System;
using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events receiver.
    /// </summary>
    public class AzureServiceBusTopicEventReceiverConfig : AzureServiceBusEventReceiverConfigBase, IValidableConfig
    {
        private string _managementConnectionString;
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
        ///     The <see cref="TimeSpan"/> idle interval after which the subscription is automatically deleted.
        /// </summary>
        /// <remarks>The minimum duration is 5 minutes. Default value is <see cref="TimeSpan.MaxValue"/>.</remarks>
        public TimeSpan SubscriptionsAutoDeleteOnIdleTimeout { get; set; } = TimeSpan.MaxValue;

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
            if (ManagementConnectionString == null)
                throw new ManagementConnectionStringIsNullException();
            if (TopicPath == null)
                throw new TopicPathIsNullException();
        }
    }
}