using System;

namespace FluentEvents.Config
{
    public class BuilderBase
    {
        protected internal EventsContext EventsContext { get; }
        protected internal IServiceProvider ServiceProvider { get; }

        public BuilderBase(EventsContext eventsContext, IServiceProvider serviceProvider)
        {
            EventsContext = eventsContext;
            ServiceProvider = serviceProvider;
        }
    }
}