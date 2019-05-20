namespace FluentEvents.Utils
{
    /// <summary>
    ///     An exception thrown when trying to select an event with a return type different from void or Task.
    /// </summary>
    public class SelectedEventHasUnsupportedReturnTypeException : FluentEventsException
    {
        /// <summary>
        ///     Creates a new instance of <see cref="SelectedEventHasUnsupportedReturnTypeException"/>
        /// </summary>
        public SelectedEventHasUnsupportedReturnTypeException() 
            : base("The return type of selected events can only be Task or void.")
        {
            
        }
    }
}