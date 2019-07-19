using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

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
        
        private SourceModelEventField(
            FieldInfo fieldInfo,
            EventInfo eventInfo,
            Type returnType,
            IReadOnlyList<ParameterExpression> eventHandlerParameters
        )
        {
            FieldInfo = fieldInfo;
            EventInfo = eventInfo;
            ReturnType = returnType;
            EventHandlerParameters = eventHandlerParameters;
        }

        internal static SourceModelEventField CreateIfValid(FieldInfo fieldInfo, EventInfo eventInfo)
        {
            var invokeMethod = GetInvokeMethod(fieldInfo);

            if (!IsReturnTypeValid(invokeMethod))
                return null;

            var eventHandlerParameters = invokeMethod
                .GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType))
                .ToArray();

            if (eventHandlerParameters.Length != 1)
                return null;

            return new SourceModelEventField(fieldInfo, eventInfo, invokeMethod.ReturnType, eventHandlerParameters);
        }

        private static bool IsReturnTypeValid(MethodInfo invokeMethod)
        {
            return invokeMethod.ReturnType == typeof(void) || invokeMethod.ReturnType == typeof(Task);
        }

        private static MethodInfo GetInvokeMethod(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType.GetMethod(nameof(Action.Invoke));
        }
    }
}