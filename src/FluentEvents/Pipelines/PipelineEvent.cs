
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
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local The setter is needed for serialization.
        public object Event { get; private set; }

        /// <summary>
        ///     The instance of the event.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local The setter is needed for serialization.
        public Type EventType { get; private set; }

        /// <summary>
        ///     Empty constructor for serialization.
        /// </summary>
        public PipelineEvent()
        {
        }

        /// <param name="e">The instance of the event.</param>
        public PipelineEvent(object e)
        {
            Event = e ?? throw new ArgumentNullException(nameof(e));
            EventType = e.GetType();
        }
    }
}