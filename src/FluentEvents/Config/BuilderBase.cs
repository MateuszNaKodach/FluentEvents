using System;

namespace FluentEvents.Config
{
    public abstract class BuilderBase
    {
        protected EventsContext EventsContext { get; }
        protected IServiceProvider ServiceProvider { get; }

        protected BuilderBase(EventsContext eventsContext, IServiceProvider serviceProvider)
        {
            EventsContext = eventsContext;
            ServiceProvider = serviceProvider;
        }
    }
}