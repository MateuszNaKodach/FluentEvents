using System;

namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     An exception thrown when the transmission plugin is not configured in the <see cref="EventsContextOptions"/>.
    /// </summary>
    [Serializable]
    public class EventTransmissionPluginIsNotConfiguredException : FluentEventsException
    {
        internal EventTransmissionPluginIsNotConfiguredException()
            : base($"The transmission plugin is not configured in the {nameof(EventsContextOptions)}")
        {
            
        }
    }
}