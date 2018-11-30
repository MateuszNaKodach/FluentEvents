using System.Text;

namespace FluentEvents.Azure.ServiceBus
{
    public interface ITopicEventSenderConfig
    {
        string ConnectionString { get; }
        Encoding Encoding { get; }
    }
}