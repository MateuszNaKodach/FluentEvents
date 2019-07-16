using System;
using FluentEvents.Infrastructure;

namespace FluentEvents
{
    /// <summary>
    ///     The EventsContext provides the API surface to configure how events are handled and to create global subscriptions.
    ///     An EventsContext should be treated as a singleton.
    /// </summary>
    public interface IEventsContext : IInfrastructure<IServiceProvider>
    {
    }
}