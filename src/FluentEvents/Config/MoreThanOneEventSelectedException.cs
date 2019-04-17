using System;

namespace FluentEvents.Config
{
    /// <summary>
    ///     An exception thrown when selecting more than one event in a selection <see cref="Action"/>.
    /// </summary>
    public class MoreThanOneEventSelectedException : FluentEventsException
    {
    }
}