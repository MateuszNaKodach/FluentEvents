namespace FluentEvents.Model
{
    /// <summary>
    ///     An exception thrown when trying to route an event with a source type different from
    ///     the <see cref="SourceModel.ClrType"/>.
    /// </summary>
    public class SourceDoesNotMatchModelTypeException : FluentEventsException
    {
        internal SourceDoesNotMatchModelTypeException()
            : base($"The event source type doesn't match the {nameof(SourceModel)}.{nameof(SourceModel.ClrType)}")
        {
            
        }
    }
}