using System;
using System.Collections.Generic;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Utils;

namespace FluentEvents.Routing
{
    /// <inheritdoc />
    internal class AttachingService : IAttachingService
    {
        private readonly ISourceModelsService _sourceModelsService;
        private readonly IForwardingService _forwardingService;
        private readonly IEnumerable<IAttachingInterceptor> _attachingInterceptors;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public AttachingService(
            ISourceModelsService sourceModelsService,
            IForwardingService forwardingService,
            IEnumerable<IAttachingInterceptor> attachingInterceptors
        )
        {
            _sourceModelsService = sourceModelsService;
            _forwardingService = forwardingService;
            _attachingInterceptors = attachingInterceptors;
        }

        /// <inheritdoc />
        public void Attach(object source, IEventsScope eventsScope)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            foreach (var attachingInterceptor in _attachingInterceptors)
                attachingInterceptor.OnAttaching(this, source, eventsScope);

            var sourceType = source.GetType();

            foreach (var baseSourceType in sourceType.GetBaseTypesAndInterfacesInclusive())
            {
                var sourceModel = _sourceModelsService.GetOrCreateSourceModel(baseSourceType);

                _forwardingService.ForwardEventsToRouting(sourceModel, source, eventsScope);
            }
        }
    }
}