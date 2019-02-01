using System;
using FluentEvents.Plugins;

namespace FluentEvents.Infrastructure
{
    internal interface IInternalServiceCollection
    {
        IServiceProvider BuildServiceProvider(EventsContext eventsContext, IFluentEventsPluginOptions options);
    }
}