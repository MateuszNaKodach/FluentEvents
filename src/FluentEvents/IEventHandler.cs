using System.Threading.Tasks;

namespace FluentEvents
{
    /// <summary>
    ///     An interface implemented by a service to handle an event.
    ///     Note: the subscription should be configured in the
    ///     <see cref="EventsContext.OnBuildingSubscriptions"/> method.
    /// </summary>
    /// <typeparam name="TSource">The type of the event source.</typeparam>
    /// <typeparam name="TEventArgs">the type of the event arguments.</typeparam>
    public interface IEventHandler<in TSource, in TEventArgs>
        where TSource : class
        where TEventArgs : class
    {
        /// <summary>
        ///     Handles an event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The arguments of the events.</param>
        /// <returns></returns>
        Task HandleEventAsync(TSource source, TEventArgs args);
    }
}
