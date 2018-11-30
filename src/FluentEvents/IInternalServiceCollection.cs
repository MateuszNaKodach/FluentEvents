using System;
using FluentEvents.Plugins;

namespace FluentEvents
{
    internal interface IInternalServiceCollection
    {
        IServiceProvider BuildServiceProvider(EventsContext eventsContext, IFluentEventsPluginOptions options);
    }
}