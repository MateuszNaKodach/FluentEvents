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

        private const BindingFlags HandlerFieldsBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;

        internal SourceModelEventField(Type clrType, EventInfo eventInfo)
        {
            EventInfo = eventInfo ?? throw new ArgumentNullException(nameof(eventInfo));
            FieldInfo = GetEventFieldInfo(clrType, EventInfo);
            ReturnType = FieldInfo.FieldType.GetMethod(nameof(Func<object>.Invoke)).ReturnType;
            
            if (ReturnType != typeof(void) && ReturnType != typeof(Task))
                throw new InvalidEventHandlerReturnTypeException();

            Pipelines = new List<IPipeline>();

            EventHandlerParameters = eventInfo.EventHandlerType
                .GetMethod(nameof(EventHandler.Invoke))
                .GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType))
                .ToArray();

            if (EventHandlerParameters.Count != 2)
                throw new InvalidEventHandlerArgsException();
        }

        private static FieldInfo GetEventFieldInfo(Type type, EventInfo eventInfo)
        {
            var eventFieldInfo = type.GetFieldFromBaseTypesInclusive(eventInfo.Name, HandlerFieldsBindingFlags);

            if (eventFieldInfo == null)
                throw new EventFieldNotFoundException();

            return eventFieldInfo;
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