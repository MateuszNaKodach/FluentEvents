using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Utils;

namespace FluentEvents.Routing
{
    public class AttachingService : IAttachingService
    {
        private readonly ITypesResolutionService m_TypesResolutionService;
        private readonly ISourceModelsService m_SourceModelsService;
        private readonly IEventsRoutingService m_EventsRoutingService;

        public AttachingService(
            ITypesResolutionService typesResolutionService,
            ISourceModelsService sourceModelsService,
            IEventsRoutingService eventsRoutingService
        )
        {
            m_TypesResolutionService = typesResolutionService;
            m_SourceModelsService = sourceModelsService;
            m_EventsRoutingService = eventsRoutingService;
        }

        public void Attach(object source, EventsScope eventsScope)
        {
            var sourceType = m_TypesResolutionService.GetSourceType(source);
            foreach (var type in sourceType.GetBaseTypesInclusive())
            {
                var sourceModel = m_SourceModelsService.GetSourceModel(type);
                if (sourceModel == null)
                    continue;

                sourceModel.ForwardEventsToRouting(source, m_EventsRoutingService, eventsScope);
                break;
            }
        }
    }
}