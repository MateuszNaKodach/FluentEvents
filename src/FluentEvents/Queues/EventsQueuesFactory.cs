namespace FluentEvents.Queues
{
    public class EventsQueuesFactory : IEventsQueuesFactory
    {
        public IEventsQueue GetNew(string queueName)
        {
            return new EventsQueue(queueName);
        }
    }
}