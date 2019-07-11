using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Model
{
    /// <summary>
    ///     Represents a field of an event source.
    /// </summary>
    public class SourceModelEventField
    {
        /// <summary>
        ///     The <see cref="System.Reflection.EventInfo"/> of the represented event field.
        /// </summary>
        public EventInfo EventInfo { get; }

        /// <summary>
        ///     The <see cref="System.Reflection.FieldInfo"/> of the represented event field.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        ///     The name of the represented event field.
        /// </summary>
        public string Name => FieldInfo.Name;

        /// <summary>
        ///     The return type of the represented event field.
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        ///     Indicates if the represented event field has an async <see cref="Delegate"/>.
        /// </summary>
        public bool IsAsync => ReturnType == typeof(Task);

        internal IReadOnlyList<ParameterExpression> EventHandlerParameters { get; }
        
        internal SourceModelEventField(FieldInfo fieldInfo, EventInfo eventInfo)
        {
            FieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
            EventInfo = eventInfo;
            ReturnType = GetInvokeMethod(fieldInfo).ReturnType;
            
            if (ReturnType != typeof(void) && ReturnType != typeof(Task))
                throw new InvalidEventHandlerReturnTypeException();

            EventHandlerParameters = GetInvokeMethod(fieldInfo)
                .GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType))
                .ToArray();

            if (EventHandlerParameters.Count != 1)
                throw new InvalidEventHandlerParametersException();
        }

        private static MethodInfo GetInvokeMethod(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType
                .GetMethod(nameof(EventHandler.Invoke));
        }
    }
}