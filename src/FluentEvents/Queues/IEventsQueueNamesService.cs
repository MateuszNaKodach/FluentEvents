namespace FluentEvents.Queues
{
    internal interface IEventsQueueNamesService
    {
        void RegisterQueueNameIfNotExists(string queueName);
        bool IsQueueNameExisting(string queueName);
    }
}