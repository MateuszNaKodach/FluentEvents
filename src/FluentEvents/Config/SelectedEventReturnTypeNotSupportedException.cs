namespace FluentEvents.Config
{
    /// <summary>
    ///     An exception thrown when trying to select an event with a return type different from void or Task.
    /// </summary>
    public class SelectedEventReturnTypeNotSupportedException : InvalidEventSelectionException
    {
        /// <summary>
        ///     Creates a new instance of <see cref="SelectedEventReturnTypeNotSupportedException"/>
        /// </summary>
        public SelectedEventReturnTypeNotSupportedException() 
            : base("The return type of selected events can only be Task or void.")
        {
            
        }
    }
}