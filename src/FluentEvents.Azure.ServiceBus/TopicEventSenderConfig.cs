using System.Text;

namespace FluentEvents.Azure.ServiceBus
{
    public class TopicEventSenderConfig : ITopicEventSenderConfig
    {
        public string ConnectionString { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}