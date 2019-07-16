using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    /// <summary>
    ///     The EventsContext provides the API surface to configure how events are handled and to create global subscriptions.
    ///     An EventsContext should be treated as a singleton.
    /// </summary>
    internal class InternalEventsContext : IInfrastructure<IServiceProvider>
    {
        private readonly Action<EventsContextOptions> _onConfiguring;
        private readonly Action<PipelinesBuilder> _onBuildingPipelines;
        private readonly Action<SubscriptionsBuilder> _onBuildingSubscriptions;

        IServiceProvider IInfrastructure<IServiceProvider>.Instance => InternalServiceProvider;

        private IInternalServiceCollection _internalServices;

        private EventsContextOptions _options;
        private IServiceProvider _internalServiceProvider;
        private IEventsContextDependencies _dependencies;
        private bool _isConfigured;

        internal bool IsInitializing => _internalServiceProvider != null && _dependencies == null;


        private IServiceProvider InternalServiceProvider
        {
            get
            {
                if (!_isConfigured)
                    throw new EventsContextIsNotConfiguredException();

                if (_internalServiceProvider == null)
                {
                    _onConfiguring(_options);
                    _internalServiceProvider = _internalServices.BuildServiceProvider(this, _options);
                    Build();
                }

                return _internalServiceProvider;
            }
        }

        internal IEventsContextDependencies Dependencies =>
            _dependencies ??
            (_dependencies = InternalServiceProvider.GetRequiredService<IEventsContextDependencies>());

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
            _internalServices = new InternalServiceCollection(new AppServiceProvider(appServiceProvider));
            _isConfigured = true;
        }

        /// <summary>
        ///     This constructor can be used when the <see cref="EventsContext" /> is configured with
        ///     the <see cref="ServiceCollectionExtensions.AddEventsContext{T}(IServiceCollection, Action{EventsContextOptions})" />
        ///     extension method.
        /// </summary>
        protected InternalEventsContext()
        {
        }

        /// <summary>
        ///     This constructor can be used when the <see cref="EventsContext" /> is not configured with
        ///     the <see cref="IServiceCollection" /> extension method.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        /// <param name="appServiceProvider">The app service provider.</param>
        protected InternalEventsContext(EventsContextOptions options, IServiceProvider appServiceProvider)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            
            if (appServiceProvider == null)
                throw new ArgumentNullException(nameof(appServiceProvider));

            _internalServices = new InternalServiceCollection(new AppServiceProvider(appServiceProvider));

            _isConfigured = true;
        }

        internal void Configure(
            EventsContextOptions options, 
            IInternalServiceCollection internalServices
        )
        {
#pragma warning disable IDE0016 // Use 'throw' expression
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (internalServices == null) throw new ArgumentNullException(nameof(internalServices));
#pragma warning restore IDE0016 // Use 'throw' expression

            _options = options;
            _internalServices = internalServices;

            _isConfigured = true;
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
