using System;

namespace FluentEvents.Config
{
    /// <summary>
    ///     An exception thrown when selecting more than one event in a selection <see cref="Action"/>.
    /// </summary>
    public class MoreThanOneEventSelectedException : FluentEventsException
    {
        internal MoreThanOneEventSelectedException()
            : base("More than one event selected." +
                   " The dynamic object provided in the selection action can only be subscribed once.")
        {
        }
    }
}