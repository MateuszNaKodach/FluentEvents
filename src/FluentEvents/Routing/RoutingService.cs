using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Utils;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Routing
{
    /// <inheritdoc />
    public class RoutingService : IRoutingService
    {
        private readonly ILogger<RoutingService> _logger;
        private readonly ISourceModelsService _sourceModelsService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public RoutingService(
            ILogger<RoutingService> logger,
            ISourceModelsService sourceModelsService
        )
        {
            _logger = logger;
            _sourceModelsService = sourceModelsService;
        }

        /// <inheritdoc />
        public async Task RouteEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope)
        {
            using (_logger.BeginEventRoutingScope(pipelineEvent))
            {
                var originalSenderType = pipelineEvent.OriginalSenderType;
                foreach (var baseSenderType in originalSenderType.GetBaseTypesInclusive())
                {
                    var field = _sourceModelsService.GetSourceModel(baseSenderType)?.GetEventField(pipelineEvent.OriginalEventFieldName);
                    if (field == null)
                        continue;

                    foreach (var pipeline in field.Pipelines)
                    {
                        _logger.EventRoutedToPipeline();
                        await pipeline.ProcessEventAsync(pipelineEvent, eventsScope).ConfigureAwait(false);
                    }

                    if (field.Pipelines.Any())
                        break;
                }
            }
        }
    }
}
