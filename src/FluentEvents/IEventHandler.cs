using System.Threading.Tasks;

namespace FluentEvents
{
    /// <summary>
    ///     An interface implemented by a service to handle an event.
    ///     Note: the subscription should be configured in the
    ///     <see cref="EventsContext.OnBuildingSubscriptions"/> method.
    /// </summary>
    /// <typeparam name="TDomainEvent">the type of the event arguments.</typeparam>
    public interface IEventHandler<in TDomainEvent>
        where TDomainEvent : class
    {
        /// <summary>
        ///     Handles an event.
        /// </summary>
        /// <param name="domainEvent">The arguments of the events.</param>
        /// <returns></returns>
        Task HandleEventAsync(TDomainEvent domainEvent);
    }
}
