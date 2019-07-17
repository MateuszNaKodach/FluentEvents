using System.Threading.Tasks;

namespace FluentEvents
{
    /// <summary>
    ///     Represents a method that publishes events with FluentEvents.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public delegate Task AsyncEventPublisher<in TEvent>(TEvent e) where TEvent : class;
}
