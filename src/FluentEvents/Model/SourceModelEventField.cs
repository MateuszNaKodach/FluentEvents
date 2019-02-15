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
        public ICollection<IPipeline> Pipelines { get; }
        public Type ReturnType { get; }
        public bool IsAsync => ReturnType == typeof(Task);

        private const BindingFlags HandlerFieldsBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;

        internal SourceModelEventField(Type clrType, EventInfo eventInfo)
        {
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

            Pipelines = new List<IPipeline>();
        }

        private static FieldInfo GetEventFieldInfo(Type type, EventInfo eventInfo)
        {
            var eventFieldInfo = type.GetFieldFromBaseTypesInclusive(eventInfo.Name, HandlerFieldsBindingFlags);

            if (eventFieldInfo == null)
                throw new EventFieldNotFoundException();

            return eventFieldInfo;
        }

        public IPipeline AddPipeline(IPipeline pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            Pipelines.Add(pipeline);
            return pipeline;
        }
    }
}