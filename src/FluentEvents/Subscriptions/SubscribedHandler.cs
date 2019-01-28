using System;

namespace FluentEvents.Subscriptions
{
    public class SubscribedHandler
    {
        public Delegate EventsHandler { get; }
        public string EventName { get; }

        public SubscribedHandler(string eventName, Delegate eventsHandler)
        {
            EventName = eventName;
            EventsHandler = eventsHandler;
        }
    }
}