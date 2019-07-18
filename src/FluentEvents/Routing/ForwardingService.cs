using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Routing
{
    /// <inheritdoc />
    internal class ForwardingService : IForwardingService
    {
        private readonly IRoutingService _routingService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public ForwardingService(IRoutingService routingService)
        {
            _routingService = routingService;
        }

        /// <inheritdoc />
        public void ForwardEventsToRouting(SourceModel sourceModel, object source, IEventsScope eventsScope)
        {
            if (!sourceModel.ClrType.IsInstanceOfType(source))
                throw new SourceDoesNotMatchModelTypeException();

            foreach (var eventField in sourceModel.EventFields)
            {
                void HandlerAction(object @event) =>
                    HandlerActionAsync(@event).GetAwaiter().GetResult();

                Task HandlerActionAsync(object @event) =>
                    _routingService.RouteEventAsync(new PipelineEvent(@event), eventsScope);

                var eventHandler = eventField.IsAsync
                    ? sourceModel.CreateEventHandler<Func<object, Task>>(eventField, HandlerActionAsync)
                    : sourceModel.CreateEventHandler<Action<object>>(eventField, HandlerAction);

                eventField.EventInfo.AddEventHandler(source, eventHandler);
            }
        }
    }
}
