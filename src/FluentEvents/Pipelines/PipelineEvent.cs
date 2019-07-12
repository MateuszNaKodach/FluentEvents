
using System;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     Represents an event.
    /// </summary>
    public class PipelineEvent
    {
        /// <summary>
        ///     The instance of the event.
        /// </summary>
        public object Event { get; }

        /// <summary>
        ///     The instance of the event.
        /// </summary>
        public Type EventType { get; }

        /// <param name="event">The instance of the event.</param>
        public PipelineEvent(object @event)
        {
            Event = @event;
            EventType = @event.GetType();
        }
    }
}