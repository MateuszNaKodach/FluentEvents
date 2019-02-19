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
        private readonly ILogger<RoutingService> m_Logger;
        private readonly ISourceModelsService m_SourceModelsService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public RoutingService(
            ILogger<RoutingService> logger,
            ISourceModelsService sourceModelsService
        )
        {
            m_Logger = logger;
            m_SourceModelsService = sourceModelsService;
        }

        /// <inheritdoc />
        public async Task RouteEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope)
        {
            using (m_Logger.BeginEventRoutingScope(pipelineEvent))
            {
                var originalSenderType = pipelineEvent.OriginalSender.GetType();
                foreach (var baseSenderType in originalSenderType.GetBaseTypesInclusive())
                {
                    var field = m_SourceModelsService.GetSourceModel(baseSenderType)?.GetEventField(pipelineEvent.OriginalEventFieldName);
                    if (field == null)
                        continue;

                    foreach (var pipeline in field.Pipelines)
                    {
                        m_Logger.EventRoutedToPipeline();
                        await pipeline.ProcessEventAsync(pipelineEvent, eventsScope);
                    }

                    if (field.Pipelines.Any())
                        break;
                }
            }
        }
    }
}
