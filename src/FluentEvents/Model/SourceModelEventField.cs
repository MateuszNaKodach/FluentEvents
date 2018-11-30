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
    public class SourceModelEventField
    {
        public EventInfo EventInfo { get; }
        public FieldInfo FieldInfo { get; }
        public string Name => FieldInfo.Name;
        public IReadOnlyList<ParameterExpression> EventHandlerParameters { get; }
        public ICollection<Pipeline> Pipelines { get; }
        public Type ReturnType { get; }
        public bool IsAsync => ReturnType == typeof(Task);

        private readonly SourceModel m_SourceModel;
        private const BindingFlags HandlerFieldsBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;

        internal SourceModelEventField(SourceModel sourceModel, Type clrType, EventInfo eventInfo)
        {
            m_SourceModel = sourceModel;
            EventInfo = eventInfo ?? throw new ArgumentNullException(nameof(eventInfo));
            EventHandlerParameters = eventInfo.EventHandlerType
                .GetMethod(nameof(EventHandler.Invoke))
                .GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType))
                .ToArray();

            if (EventHandlerParameters.Count != 2)
                throw new InvalidEventHandlerArgsException();

            FieldInfo = GetEventFieldInfo(clrType, EventInfo);
            ReturnType = FieldInfo.FieldType.GetMethod(nameof(Func<object>.Invoke)).ReturnType;
            
            if (ReturnType != typeof(void) && ReturnType != typeof(Task))
                throw new InvalidEventHandlerReturnTypeException();

            Pipelines = new List<Pipeline>();
        }

        private static FieldInfo GetEventFieldInfo(Type type, EventInfo eventInfo)
        {
            var eventFieldInfo = type.GetFieldFromBaseTypesInclusive(eventInfo.Name, HandlerFieldsBindingFlags);

            if (eventFieldInfo == null)
                throw new EventFieldNotFoundException();

            return eventFieldInfo;
        }

        public Pipeline AddEventPipelineConfig(string queueName)
        {
            var pipeline = new Pipeline(
                queueName,
                m_SourceModel.EventsContext,
                ((IInternalServiceProvider)m_SourceModel.EventsContext).InternalServiceProvider
            );

            Pipelines.Add(pipeline);
            return pipeline;
        }
    }
}