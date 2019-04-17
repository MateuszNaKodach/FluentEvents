using System.Threading.Tasks;

namespace FluentEvents.Model
{
    /// <summary>
    ///     An exception that is thrown when trying to create an event on a <see cref="SourceModel"/>
    ///     that have an invalid return type (Supported return types are void or <see cref="Task"/>).
    /// </summary>
    public class InvalidEventHandlerReturnTypeException : FluentEventsException
    {
        internal InvalidEventHandlerReturnTypeException() 
            : base("The event handler return type is different from Task or void.")
        {
            
        }
    }
}