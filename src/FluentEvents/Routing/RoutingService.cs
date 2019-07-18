using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Routing
{
    /// <inheritdoc />
    internal class RoutingService : IRoutingService
    {
        private readonly ILogger<RoutingService> _logger;
        private readonly IPipelinesService _pipelinesService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public RoutingService(
            ILogger<RoutingService> logger,
            IPipelinesService pipelinesService
        )
        {
            _logger = logger;
            _pipelinesService = pipelinesService;
        }

        /// <inheritdoc />
        public async Task RouteEventAsync(PipelineEvent pipelineEvent, IEventsScope eventsScope)
        {
            using (_logger.BeginEventRoutingScope(pipelineEvent))
            {
                var pipelines = _pipelinesService.GetPipelines(pipelineEvent.EventType);

                foreach (var pipeline in pipelines)
                {
                    _logger.EventRoutedToPipeline();

                    await pipeline.ProcessEventAsync(pipelineEvent, eventsScope).ConfigureAwait(false);
                }
            }
        }
    }
}
