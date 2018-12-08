using System.Collections;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    public interface IEventsQueueNamesService
    {
        void RegisterQueueNameIfNotExists(string queueName);
        bool IsQueueNameExisting(string queueName);
    }
}