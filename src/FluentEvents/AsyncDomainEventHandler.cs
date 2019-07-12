using System.Threading.Tasks;

namespace FluentEvents
{
    /// <summary>
    ///     Represents the method that will handle a domain event.
    /// </summary>
    /// <param name="domainEvent">The event.</param>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    public delegate Task AsyncDomainEventHandler<in TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class;
}
