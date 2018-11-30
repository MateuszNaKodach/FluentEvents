namespace FluentEvents.Queues
{
    public interface IEventsQueuesFactory
    {
        IEventsQueue GetNew(string queueName);
    }
}