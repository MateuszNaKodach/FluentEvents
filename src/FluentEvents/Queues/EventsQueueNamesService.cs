using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentEvents.Queues
{
    public class EventsQueueNamesService : IEventsQueueNamesService
    {
        private readonly IList<string> m_QueueNames;

        public EventsQueueNamesService()
        {
            m_QueueNames = new List<string>();
        }

        public void RegisterQueueNameIfNotExists(string queueName)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            m_QueueNames.Add(queueName);
        }

        public bool IsQueueNameExisting(string queueName)
        {
            return m_QueueNames.Any(x => x == queueName);
        }
    }
}
