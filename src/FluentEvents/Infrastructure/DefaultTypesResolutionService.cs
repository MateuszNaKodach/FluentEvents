using System;

namespace FluentEvents.Infrastructure
{
    public class DefaultTypesResolutionService : ITypesResolutionService
    {
        public Type GetSourceType(object source) => source.GetType();

        public Type GetEventArgsType(object eventArgs) => eventArgs.GetType();
    }
}