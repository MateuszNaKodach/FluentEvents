using FluentEvents.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Infrastructure
{
    internal interface IInternalServiceCollection
    {
        ServiceProvider BuildServiceProvider(InternalEventsContext eventsContext, IFluentEventsPluginOptions options);
    }
}