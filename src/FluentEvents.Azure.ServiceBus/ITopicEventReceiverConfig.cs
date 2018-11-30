using System;
using System.Text;

namespace FluentEvents.Azure.ServiceBus
{
    public interface ITopicEventReceiverConfig
    {
        string TopicPath { get; }
        string ManagementConnectionString { get; }
        string ReceiveConnectionString { get; }
        TimeSpan SubscriptionsAutoDeleteOnIdleTimeout { get; }
        int MaxConcurrentMessages { get; }
        Func<string> SubscriptionNameGenerator { get; }
        Encoding Encoding { get; }
    }
}