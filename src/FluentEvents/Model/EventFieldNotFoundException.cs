namespace FluentEvents.Model
{
    /// <summary>
    ///     An exception thrown when trying to get or create an event on a <see cref="SourceModel"/>
    ///     that doesn't exist on the <see cref="SourceModel.ClrType"/>.
    /// </summary>
    public class EventFieldNotFoundException : FluentEventsException
    {
    }
}
