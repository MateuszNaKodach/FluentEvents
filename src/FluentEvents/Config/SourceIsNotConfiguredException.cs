using System;

namespace FluentEvents.Config
{
    public class SourceIsNotConfiguredException : FluentEventsException
    {
        public SourceIsNotConfiguredException(Type sourceType) 
            : base($"Events source with type \"{sourceType.FullName}\" is not configured")
        {
            
        }
    }
}