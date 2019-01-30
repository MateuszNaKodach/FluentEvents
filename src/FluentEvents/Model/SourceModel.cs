using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentEvents.Model
{
    public class SourceModel
    {
        public Type ClrType { get; }
        public IEnumerable<SourceModelEventField> EventFields => m_EventFields;

        private readonly IList<SourceModelEventField> m_EventFields;

        public SourceModel(Type clrType)
        {
            ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
            m_EventFields = new List<SourceModelEventField>();
        }
        
        public SourceModelEventField GetOrCreateEventField(string name)
        {
            var eventField = m_EventFields.FirstOrDefault(x => x.Name == name);
            if (eventField == null)
            {
                var eventInfo = ClrType.GetEvents().FirstOrDefault(x => x.Name == name);
                if (eventInfo == null)
                    throw new EventFieldNotFoundException();

                eventField = new SourceModelEventField(ClrType, eventInfo);
                m_EventFields.Add(eventField);
            }

            return eventField;
        }

        public SourceModelEventField GetEventField(string name)
            => m_EventFields.FirstOrDefault(x => x.Name == name);

        internal Delegate CreateEventHandler<T>(SourceModelEventField eventField, T handlerAction)
        {
            var invokeMethodInfo = typeof(T).GetMethod(nameof(Action.Invoke));

            var handler = Expression.Lambda(
                    eventField.EventInfo.EventHandlerType,
                    Expression.Call(
                        Expression.Constant(handlerAction),
                        invokeMethodInfo,
                        eventField.EventHandlerParameters[0],
                        eventField.EventHandlerParameters[1]),
                    eventField.EventHandlerParameters
                )
                .Compile();

            return handler;
        }
    }
}