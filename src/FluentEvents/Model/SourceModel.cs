using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentEvents.Utils;

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
        ///     The list of the fields present on the <see cref="ClrType"/>.
        /// </summary>
        public FieldInfo[] ClrTypeEventFieldInfos { get; }

        /// <summary>
        ///     The list of the event fields present on the <see cref="ClrType"/> and created on this model.
        /// </summary>
        public IEnumerable<SourceModelEventField> EventFields => _eventFields;

        private readonly IList<SourceModelEventField> _eventFields;
        private const BindingFlags HandlerFieldsBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;

        /// <summary>
        ///     Creates a new instance of a <see cref="SourceModel"/>.
        /// </summary>
        /// <param name="clrType"></param>
        public SourceModel(Type clrType)
        {
            ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
            ClrTypeEventFieldInfos = ClrType.GetEvents().Select(x => GetEventFieldInfo(clrType, x)).ToArray();
            _eventFields = new List<SourceModelEventField>();
        }

        /// <summary>
        ///     Gets or creates an event field if present on the <see cref="ClrType"/>.
        /// </summary>
        /// <param name="name">The name of the event field.</param>
        public SourceModelEventField GetOrCreateEventField(string name)
        {
            var eventField = _eventFields.FirstOrDefault(x => x.Name == name);
            if (eventField == null)
            {
                var fieldInfo = ClrTypeEventFieldInfos.FirstOrDefault(x => x.Name == name);
                if (fieldInfo == null)
                    throw new EventFieldNotFoundException();

                var eventInfo = ClrType.GetEvent(fieldInfo.Name);
                eventField = new SourceModelEventField(fieldInfo, eventInfo);
                _eventFields.Add(eventField);
            }

            return eventField;
        }

        /// <summary>
        ///     Gets an event field previously created with <see cref="GetOrCreateEventField"/>
        /// </summary>
        /// <param name="name">The name of the event field.</param>
        public SourceModelEventField GetEventField(string name)
            => _eventFields.FirstOrDefault(x => x.Name == name);

        internal Delegate CreateEventHandler<T>(SourceModelEventField eventField, T handlerAction)
        {
            var invokeMethodInfo = typeof(T).GetMethod(nameof(Action.Invoke));

            var handler = Expression.Lambda(
                    eventField.FieldInfo.FieldType,
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

        private static FieldInfo GetEventFieldInfo(Type type, MemberInfo eventInfo)
        {
            return type.GetFieldFromBaseTypesInclusive(eventInfo.Name, HandlerFieldsBindingFlags);
        }
    }
}