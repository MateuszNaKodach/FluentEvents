using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentEvents.Queues
{
    internal class EventsQueueNamesService : IEventsQueueNamesService
    {
        private readonly IList<string> _queueNames;

        public EventsQueueNamesService()
        {
            _queueNames = new List<string>();
        }

        public void RegisterQueueNameIfNotExists(string queueName)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            _queueNames.Add(queueName);
        }

        public bool IsQueueNameExisting(string queueName)
        {
            return _queueNames.Any(x => x == queueName);
        }
    }
}
