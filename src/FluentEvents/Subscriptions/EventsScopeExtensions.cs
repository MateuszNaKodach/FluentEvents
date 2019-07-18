using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal static class EventsScopeExtensions
    {
        public static IEventsScopeSubscriptionsFeature GetSubscriptionsFeature(this IEventsScope eventsScope)
        {
            return eventsScope.GetOrAddFeature<IEventsScopeSubscriptionsFeature>(
                x => new EventsScopeSubscriptionsFeature(x)
            );
        }
    }
}
