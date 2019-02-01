using System;
using System.Data.Entity.Core.Objects;
using FluentEvents.Infrastructure;

namespace FluentEvents.EntityFramework
{
    public class EntityFrameworkTypesResolutionService : ITypesResolutionService
    {
        public Type GetSourceType(object source)
        {
            return ObjectContext.GetObjectType(source.GetType());
        }

        public Type GetEventArgsType(object eventArgs)
        {
            return eventArgs.GetType();
        }
    }
}