using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentEvents.Model
{
    /// <summary>
    ///     Represents an event source.
    /// </summary>
    public class SourceModel
    {
        /// <summary>
        ///     The type of the event source.
        /// </summary>
        public Type ClrType { get; }

        /// <summary>
        ///     The list of the event fields present on the <see cref="ClrType"/> and created on this model.
        /// </summary>
        public IEnumerable<SourceModelEventField> EventFields => m_EventFields;

        private readonly IList<SourceModelEventField> m_EventFields;

        /// <summary>
        ///     Creates a new instance of a <see cref="SourceModel"/>.
        /// </summary>
        /// <param name="clrType"></param>
        public SourceModel(Type clrType)
        {
            ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
            m_EventFields = new List<SourceModelEventField>();
        }

        /// <summary>
        ///     Gets or creates an event field if present on the <see cref="ClrType"/>.
        /// </summary>
        /// <param name="name">The name of the event field.</param>
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

        /// <summary>
        ///     Gets an event field previously created with <see cref="GetOrCreateEventField"/>
        /// </summary>
        /// <param name="name">The name of the event field.</param>
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