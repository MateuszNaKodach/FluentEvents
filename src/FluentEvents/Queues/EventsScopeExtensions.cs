using FluentEvents.Infrastructure;

namespace FluentEvents.Queues
{
    internal static class EventsScopeExtensions
    {
        public static IEventsScopeQueuesFeature GetQueuesFeature(this IEventsScope eventsScope)
        {
            return eventsScope.GetOrAddFeature<IEventsScopeQueuesFeature>(x => new EventsScopeQueuesFeature());
        }
    }
}
