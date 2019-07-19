using System;

namespace FluentEvents.Plugins
{
    /// <summary>
    ///     An exception thrown when adding the same plugin twice.
    /// </summary>
    [Serializable]
    public class DuplicatePluginException : FluentEventsException
    {
        internal DuplicatePluginException() 
            : base("The plugin being added was already configured.")
        {
            
        }
    }
}