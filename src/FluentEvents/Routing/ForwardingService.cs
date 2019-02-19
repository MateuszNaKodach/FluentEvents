using System;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Routing
{
    /// <inheritdoc />
    public class ForwardingService : IForwardingService
    {
        private readonly IRoutingService m_RoutingService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public ForwardingService(IRoutingService routingService)
        {
            m_RoutingService = routingService;
        }

        /// <inheritdoc />
        public void ForwardEventsToRouting(SourceModel sourceModel, object source, EventsScope eventsScope)
        {
            if (!sourceModel.ClrType.IsInstanceOfType(source))
                throw new SourceDoesNotMatchModelTypeException();

            foreach (var eventField in sourceModel.EventFields)
            {
                void HandlerAction(object sender, object args) =>
                    m_RoutingService.RouteEventAsync(
                        new PipelineEvent(sourceModel.ClrType, eventField.EventInfo.Name, sender, args),
                        eventsScope
                    ).GetAwaiter().GetResult();

                async Task AsyncHandlerAction(object sender, object args) =>
                    await m_RoutingService.RouteEventAsync(
                        new PipelineEvent(sourceModel.ClrType, eventField.EventInfo.Name, sender, args),
                        eventsScope
                    );

                var eventHandler = eventField.IsAsync
                    ? sourceModel.CreateEventHandler<Func<object, object, Task>>(eventField, AsyncHandlerAction)
                    : sourceModel.CreateEventHandler<Action<object, object>>(eventField, HandlerAction);

                eventField.EventInfo.AddEventHandler(source, eventHandler);
            }
        }
    }
}
