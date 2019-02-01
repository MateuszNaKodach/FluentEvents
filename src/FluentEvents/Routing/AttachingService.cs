using System;
using FluentEvents.Model;
using FluentEvents.Utils;

namespace FluentEvents.Routing
{
    public class AttachingService : IAttachingService
    {
        private readonly ISourceModelsService m_SourceModelsService;
        private readonly IForwardingService m_ForwardingService;

        public AttachingService(
            ISourceModelsService sourceModelsService,
            IForwardingService forwardingService
        )
        {
            m_SourceModelsService = sourceModelsService;
            m_ForwardingService = forwardingService;
        }

        public void Attach(object source, EventsScope eventsScope)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            var sourceType = source.GetType();

            foreach (var type in sourceType.GetBaseTypesInclusive())
            {
                var sourceModel = m_SourceModelsService.GetSourceModel(type);
                if (sourceModel == null)
                    continue;

                m_ForwardingService.ForwardEventsToRouting(sourceModel, source, eventsScope);
                break;
            }
        }
    }
}