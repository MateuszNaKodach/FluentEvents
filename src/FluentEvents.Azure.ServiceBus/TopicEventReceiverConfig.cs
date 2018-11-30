using System;
using System.Text;

namespace FluentEvents.Azure.ServiceBus
{
    public class TopicEventReceiverConfig : ITopicEventReceiverConfig
    {
        public string TopicPath { get; set; }
        public string ManagementConnectionString { get; set; }
        public string ReceiveConnectionString { get; set; }
        public TimeSpan SubscriptionsAutoDeleteOnIdleTimeout { get; set; } = TimeSpan.FromMinutes(10);
        public int MaxConcurrentMessages { get; set; } = 1;
        public Func<string> SubscriptionNameGenerator { get; set; } = () => Guid.NewGuid().ToString();
        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}