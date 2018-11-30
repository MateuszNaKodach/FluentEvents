using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Routing;

namespace FluentEvents.Model
{
    public class SourceModel
    {
        public EventsContext EventsContext { get; }
        public Type ClrType { get; }
        public IEnumerable<SourceModelEventField> EventFields => m_EventFields;

        private readonly IList<SourceModelEventField> m_EventFields;

        public SourceModel(Type clrType, EventsContext eventsContext)
        {
            ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
            EventsContext = eventsContext;
            m_EventFields = new List<SourceModelEventField>();
        }

        public void ForwardEventsToRouting(object source, IEventsRoutingService eventsRoutingService, EventsScope eventsScope)
        {
            if (source.GetType() != ClrType)
                throw new SourceDoesNotMatchModelTypeException();

            foreach (var eventField in m_EventFields)
            {
                void HandlerAction(object sender, object args) =>
                    eventsRoutingService.RouteEventAsync(new PipelineEvent(eventField.EventInfo.Name, sender, args), eventsScope).GetAwaiter().GetResult();

                async Task AsyncHandlerAction(object sender, object args) =>
                    await eventsRoutingService.RouteEventAsync(new PipelineEvent(eventField.EventInfo.Name, sender, args), eventsScope);

                var eventHandler = eventField.IsAsync
                    ? CreateEventHandler<Func<object, object, Task>>(eventField, AsyncHandlerAction)
                    : CreateEventHandler<Action<object, object>>(eventField, HandlerAction);

                eventField.EventInfo.AddEventHandler(source, eventHandler);
            }
        }

        public SourceModelEventField GetOrCreateEventField(string name)
        {
            var eventField = m_EventFields.FirstOrDefault(x => x.Name == name);
            if (eventField == null)
            {
                var eventInfo = ClrType.GetEvents().FirstOrDefault(x => x.Name == name);
                if (eventInfo == null)
                    throw new EventFieldNotFoundException();

                eventField = new SourceModelEventField(this, ClrType, eventInfo);
                m_EventFields.Add(eventField);
            }

            return eventField;
        }

        public SourceModelEventField GetEventField(string name)
            => m_EventFields.FirstOrDefault(x => x.Name == name);

        private Delegate CreateEventHandler<T>(SourceModelEventField eventField, T handlerAction)
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