using System;
using FluentEvents.Config;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Infrastructure
{
    internal class InternalEventsContext
    {
        private readonly EventsContextOptions _options;
        private readonly Action<EventsContextOptions> _onConfiguring;
        private readonly Action<PipelinesBuilder> _onBuildingPipelines;
        private readonly Action<SubscriptionsBuilder> _onBuildingSubscriptions;

        public IServiceProvider InternalServiceProvider { get; }

        public InternalEventsContext(
            EventsContextOptions options,
            Action<EventsContextOptions> onConfiguring,
            Action<PipelinesBuilder> onBuildingPipelines,
            Action<SubscriptionsBuilder> onBuildingSubscriptions,
            IAppServiceProvider appServiceProvider
        )
        {
            _options = options;
            _onConfiguring = onConfiguring;
            _onBuildingPipelines = onBuildingPipelines;
            _onBuildingSubscriptions = onBuildingSubscriptions;

            Configure();
            var internalServices = new InternalServiceCollection(appServiceProvider);
            InternalServiceProvider = internalServices.BuildServiceProvider(this, _options);
            Build();
        }

        private void Configure()
        {
            _onConfiguring(_options);
        }

        private void Build()
        {
            _onBuildingPipelines(InternalServiceProvider.GetRequiredService<PipelinesBuilder>());
            _onBuildingSubscriptions(InternalServiceProvider.GetRequiredService<SubscriptionsBuilder>());

            foreach (var validableConfig in InternalServiceProvider.GetServices<IValidableConfig>())
                validableConfig.Validate();
        }
    }
}
