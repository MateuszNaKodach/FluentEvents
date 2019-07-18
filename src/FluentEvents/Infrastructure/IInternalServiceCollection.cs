using FluentEvents.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Infrastructure
{
    internal interface IInternalServiceCollection
    {
        ServiceProvider BuildServiceProvider(IEventsContext eventsContext, IFluentEventsPluginOptions options);
    }
}