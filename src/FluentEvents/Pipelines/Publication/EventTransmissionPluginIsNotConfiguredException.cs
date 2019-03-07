namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     An exception thrown when the transmission plugin is not configured in the <see cref="EventsContextOptions"/>.
    /// </summary>
    public class EventTransmissionPluginIsNotConfiguredException : FluentEventsException
    {
    }
}