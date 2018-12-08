using System;
using FluentEvents.Infrastructure;

namespace FluentEvents
{
    public interface IInfrastructureEventsContext : IInfrastructure<IServiceProvider>
    {
    }
}