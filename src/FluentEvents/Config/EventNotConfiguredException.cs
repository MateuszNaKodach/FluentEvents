namespace FluentEvents.Config
{
    /// <summary>
    ///     An exception thrown when referencing an event that was not
    ///     configured inside of the <see cref="EventsContext.OnBuildingPipelines"/> method.
    /// </summary>
    public class EventNotConfiguredException : FluentEventsException
    {
    }
}