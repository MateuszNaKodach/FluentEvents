﻿using System;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Routing
{
    public class ForwardingService : IForwardingService
    {
        private readonly IRoutingService m_RoutingService;

        public ForwardingService(IRoutingService routingService)
        {
            m_RoutingService = routingService;
        }

        public void ForwardEventsToRouting(SourceModel sourceModel, object source, EventsScope eventsScope)
        {
            if (source.GetType() != sourceModel.ClrType)
                throw new SourceDoesNotMatchModelTypeException();

            foreach (var eventField in sourceModel.EventFields)
            {
                void HandlerAction(object sender, object args) =>
                    m_RoutingService.RouteEventAsync(
                        new PipelineEvent(eventField.EventInfo.Name, sender, args), eventsScope
                    ).GetAwaiter().GetResult();

                async Task AsyncHandlerAction(object sender, object args) =>
                    await m_RoutingService.RouteEventAsync(
                        new PipelineEvent(eventField.EventInfo.Name, sender, args), eventsScope
                    );

                var eventHandler = eventField.IsAsync
                    ? sourceModel.CreateEventHandler<Func<object, object, Task>>(eventField, AsyncHandlerAction)
                    : sourceModel.CreateEventHandler<Action<object, object>>(eventField, HandlerAction);

                eventField.EventInfo.AddEventHandler(source, eventHandler);
            }
        }
    }
}
