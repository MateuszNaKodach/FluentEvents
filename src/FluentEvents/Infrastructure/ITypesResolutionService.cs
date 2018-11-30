using System;

namespace FluentEvents.Infrastructure
{
    public interface ITypesResolutionService
    {
        Type GetSourceType(object source);
        Type GetEventArgsType(object eventArgs);
    }
}