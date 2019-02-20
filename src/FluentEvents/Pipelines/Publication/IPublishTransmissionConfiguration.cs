using System;

namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     The configuration for an event transmission.
    /// </summary>
    public interface IPublishTransmissionConfiguration
    {
        /// <summary>
        ///     The type of the event sender.
        /// </summary>
        Type SenderType { get; }
    }
}