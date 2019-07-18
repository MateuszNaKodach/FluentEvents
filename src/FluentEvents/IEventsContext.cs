using System;
using FluentEvents.Infrastructure;

namespace FluentEvents
{
    /// <summary>
    ///     An interface for <see cref="EventsContext"/>s.
    /// </summary>
    public interface IEventsContext : IInfrastructure<IServiceProvider>
    {
    }
}