using System;
using FluentEvents.Plugins;

namespace FluentEvents.Infrastructure
{
    internal interface IInternalServiceCollection
    {
        IServiceProvider BuildServiceProvider(InternalEventsContext eventsContext, IFluentEventsPluginOptions options);
    }
}