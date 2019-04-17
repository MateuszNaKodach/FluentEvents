using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Utils;

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
        ///     A collection of the configured pipelines for this event field.
        /// </summary>
        public ICollection<IPipeline> Pipelines { get; }

        /// <summary>
        ///     The event args type of the represented event field.
        /// </summary>
        public Type EventArgsType => EventHandlerParameters[1].Type;

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

            Pipelines = new List<IPipeline>();

            EventHandlerParameters = GetInvokeMethod(fieldInfo)
                .GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType))
                .ToArray();

            if (EventHandlerParameters.Count != 2)
                throw new InvalidEventHandlerArgsException();
        }

        private static MethodInfo GetInvokeMethod(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType
                .GetMethod(nameof(EventHandler.Invoke));
        }

        /// <summary>
        ///     Adds a pipeline to this event field.
        /// </summary>
        /// <param name="pipeline">The pipeline to add</param>
        /// <returns>The same <see cref="IPipeline"/> added.</returns>
        public IPipeline AddPipeline(IPipeline pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            Pipelines.Add(pipeline);
            return pipeline;
        }
    }
}