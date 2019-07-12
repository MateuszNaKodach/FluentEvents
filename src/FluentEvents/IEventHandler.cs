using System.Threading.Tasks;

namespace FluentEvents
{
    /// <summary>
    ///     An interface implemented by a service to handle an event.
    ///     Note: the subscription should be configured in the
    ///     <see cref="EventsContext.OnBuildingSubscriptions"/> method.
    /// </summary>
    /// <typeparam name="TDomainEvent">the type of the event.</typeparam>
    // ReSharper disable once TypeParameterCanBeVariant (Variant type parameter here would allow invalid subscription configurations)
    public interface IEventHandler<TDomainEvent>
        where TDomainEvent : class
    {
        /// <summary>
        ///     Handles an event.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns></returns>
        Task HandleEventAsync(TDomainEvent e);
    }
}
