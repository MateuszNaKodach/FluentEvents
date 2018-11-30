using System;
using FluentEvents.Infrastructure;

namespace FluentEvents.EntityFramework
{
    public class EntityFrameworkTypesResolutionService : ITypesResolutionService
    {
        public Type GetSourceType(object source)
        {
            var entityType = source.GetType();

            return entityType.Namespace == "System.Data.Entity.DynamicProxies" ? entityType.BaseType : entityType;
        }

        public Type GetEventArgsType(object eventArgs)
        {
            return eventArgs.GetType();
        }
    }
}