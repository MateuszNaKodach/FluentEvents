using System;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Utils;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Routing
{
    public class RoutingService : IRoutingService
    {
        private readonly ILogger<RoutingService> m_Logger;
        private readonly ITypesResolutionService m_TypesResolutionService;
        private readonly ISourceModelsService m_SourceModelsService;

        public RoutingService(
            ILogger<RoutingService> logger,
            ITypesResolutionService typesResolutionService,
            ISourceModelsService sourceModelsService
        )
        {
            m_Logger = logger;
            m_TypesResolutionService = typesResolutionService;
            m_SourceModelsService = sourceModelsService;
        }
        
        public async Task RouteEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope)
        {
            using (m_Logger.BeginEventRoutingScope(pipelineEvent))
            {
                var originalSenderType = m_TypesResolutionService.GetSourceType(pipelineEvent.OriginalSender);
                foreach (var baseSenderType in originalSenderType.GetBaseTypesInclusive())
                {
                    var field = m_SourceModelsService.GetSourceModel(baseSenderType)?.GetEventField(pipelineEvent.OriginalEventFieldName);
                    if (field == null)
                        continue;

                    foreach (var pipeline in field.Pipelines)
                    {
                        if (pipeline.QueueName != null)
                        {
                            m_Logger.EventRoutedToQueue(pipeline.QueueName);
                            eventsScope.EnqueueEvent(pipelineEvent, pipeline);
                        }
                        else
                        {
                            m_Logger.EventRoutedToPipeline();
                            await pipeline.ProcessEventAsync(pipelineEvent, eventsScope);
                        }
                    }

                    if (field.Pipelines.Any())
                        break;
                }
            }
        }
    }
}
