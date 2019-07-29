using System;
using FluentEvents.Configuration;
using FluentEvents.ServiceProviders;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Infrastructure
{
    internal class InternalEventsContext : IDisposable
    {
        private readonly EventsContextOptions _options;
        private readonly Action<EventsContextOptions> _onConfiguring;
        private readonly Action<IPipelinesBuilder> _onBuildingPipelines;
        private readonly Action<SubscriptionsBuilder> _onBuildingSubscriptions;
        private readonly ServiceProvider _internalServiceProvider;

        public IServiceProvider InternalServiceProvider => _internalServiceProvider;

        public InternalEventsContext(
            EventsContextOptions options,
            Action<EventsContextOptions> onConfiguring,
            Action<IPipelinesBuilder> onBuildingPipelines,
            Action<SubscriptionsBuilder> onBuildingSubscriptions,
            IRootAppServiceProvider rootAppServiceProvider
        )
        {
            _options = options;
            _onConfiguring = onConfiguring;
            _onBuildingPipelines = onBuildingPipelines;
            _onBuildingSubscriptions = onBuildingSubscriptions;

            Configure();
            var internalServices = new InternalServiceCollection(rootAppServiceProvider);
            _internalServiceProvider = internalServices.BuildServiceProvider(_options);
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
        }

        public void Dispose()
        {
            _internalServiceProvider?.Dispose();
        }
    }
}
