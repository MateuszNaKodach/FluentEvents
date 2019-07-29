using System;
using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Filters;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;
using FluentEvents.ServiceProviders;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class CombinatorialPipelineTest
    {
        private readonly string _testValue = "TestValue";

        private TestEventsContext _context;
        private TestEntity _entity;
        private SubscribingService _scopedSubscribingService;
        private SubscribingService _singletonSubscribingService;
        private IServiceProvider _serviceProvider;
        private EventsScope _eventsScope;

        private void SetUpContext(TestRunParameters testRunParameters)
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options => { });

            services.AddSingleton(testRunParameters);

            services.AddScoped<ScopedSubscribingService>();
            services.AddSingleton<SingletonSubscribingService>();
            _serviceProvider = services.BuildServiceProvider();

            _entity = new TestEntity();

            var serviceScope = _serviceProvider.CreateScope();
            _scopedSubscribingService = serviceScope.ServiceProvider.GetRequiredService<ScopedSubscribingService>();
            _singletonSubscribingService = serviceScope.ServiceProvider.GetRequiredService<SingletonSubscribingService>();
            _context = serviceScope.ServiceProvider.GetRequiredService<TestEventsContext>();
            _eventsScope = serviceScope.ServiceProvider.GetRequiredService<EventsScope>();

            _context.WatchSourceEvents(_entity, _eventsScope);
        }

        [Test]
        [Combinatorial]
        public async Task CombinePipelineModules(
            [Values] bool isAsync,
            [Values] bool isFiltered,
            [Values] bool isProjected,
            [Values] bool isQueued,
            [Values] PublicationType publicationType
        )
        {
            var testRunParameters = new TestRunParameters
            {
                IsAsync = isAsync,
                IsFiltered = isFiltered,
                IsProjected = isProjected,
                IsQueued = isQueued,
                PublicationType = publicationType
            };

            SetUpContext(testRunParameters);

            await RaiseEvent(isAsync);

            if (isQueued)
                await _context.ProcessQueuedEventsAsync(_eventsScope);

            var subscribingService = publicationType == PublicationType.ScopedWithServiceHandlerSubscription
                ? _scopedSubscribingService
                : _singletonSubscribingService;

            Assert.That(subscribingService,
                isProjected
                    ? Has.Property(nameof(SubscribingService.ProjectedTestEvents)).With.One.Items
                    : Has.Property(nameof(SubscribingService.TestEvents)).With.One.Items
            );
        }

        private async Task RaiseEvent(bool isAsync)
        {
            if (isAsync)
                await _entity.RaiseAsyncEvent(_testValue);
            else
                _entity.RaiseEvent(_testValue);
        }
        
        public enum PublicationType
        {
            GlobalWithServiceHandlerSubscription,
            ScopedWithServiceHandlerSubscription
        }

        private class TestRunParameters
        {
            public bool IsAsync { get; set; }
            public bool IsFiltered { get; set; }
            public bool IsProjected { get; set; }
            public bool IsQueued { get; set; }
            public PublicationType PublicationType { get; set; }
        }

        private sealed class TestEventsContext : EventsContext
        {
            private readonly TestRunParameters _parameters;
            
            public TestEventsContext(
                EventsContextOptions options, 
                IRootAppServiceProvider rootAppServiceProvider,
                TestRunParameters parameters
            )
                : base(options, rootAppServiceProvider)
            {
                _parameters = parameters;
            }

            protected override void OnBuildingSubscriptions(ISubscriptionsBuilder subscriptionsBuilder)
            {
                if (!_parameters.IsProjected)
                {
                    if (_parameters.PublicationType == PublicationType.ScopedWithServiceHandlerSubscription)
                        AddScopedServiceHandlerSubscription(subscriptionsBuilder);

                    if (_parameters.PublicationType == PublicationType.GlobalWithServiceHandlerSubscription)
                        AddGlobalServiceHandlerSubscription(subscriptionsBuilder);
                }

                if (_parameters.IsProjected)
                {
                    if (_parameters.PublicationType == PublicationType.ScopedWithServiceHandlerSubscription)
                        AddScopedServiceHandlerSubscriptionToProjection(subscriptionsBuilder);

                    if (_parameters.PublicationType == PublicationType.GlobalWithServiceHandlerSubscription)
                        AddGlobalServiceHandlerSubscriptionToProjection(subscriptionsBuilder);
                }
            }

            private void AddGlobalServiceHandlerSubscriptionToProjection(ISubscriptionsBuilder subscriptionsBuilder)
            {
                var serviceConfigurator = subscriptionsBuilder
                    .ServiceHandler<SingletonSubscribingService, ProjectedTestEvent>();

                serviceConfigurator.HasGlobalSubscription();
            }
            
            private void AddScopedServiceHandlerSubscriptionToProjection(ISubscriptionsBuilder subscriptionsBuilder)
            {
                var serviceConfigurator = subscriptionsBuilder
                    .ServiceHandler<ScopedSubscribingService, ProjectedTestEvent>();

                serviceConfigurator.HasScopedSubscription();
            }

            private void AddGlobalServiceHandlerSubscription(ISubscriptionsBuilder subscriptionsBuilder)
            {
                var serviceConfigurator = subscriptionsBuilder
                    .ServiceHandler<SingletonSubscribingService, TestEvent>();

                serviceConfigurator.HasGlobalSubscription();
            }

            private void AddScopedServiceHandlerSubscription(ISubscriptionsBuilder subscriptionsBuilder)
            {
                var serviceConfigurator = subscriptionsBuilder
                    .ServiceHandler<ScopedSubscribingService, TestEvent>();

                serviceConfigurator.HasScopedSubscription();
            }

            protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
            {
                var pipelineConfigurator = pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped();

                if (_parameters.IsFiltered)
                    pipelineConfigurator.ThenIsFiltered(testEvent => true);

                if (_parameters.IsProjected)
                {
                    var pipelineConfiguratorWithProjection = pipelineConfigurator.ThenIsProjected(
                        testEvent => new ProjectedTestEvent()
                    );

                    ContinueConfiguration(pipelineConfiguratorWithProjection);
                }
                else
                {
                    ContinueConfiguration(pipelineConfigurator);
                }
            }

            private void ContinueConfiguration<TEvent>(
                EventPipelineConfiguration<TEvent> eventPipelineConfiguration
            )
                where TEvent : class
            {
                if (_parameters.IsQueued)
                    eventPipelineConfiguration.ThenIsQueuedTo("DefaultQueue");

                switch (_parameters.PublicationType)
                {
                    case PublicationType.GlobalWithServiceHandlerSubscription:
                        eventPipelineConfiguration.ThenIsPublishedToGlobalSubscriptions();
                        break;
                    case PublicationType.ScopedWithServiceHandlerSubscription:
                        eventPipelineConfiguration.ThenIsPublishedToScopedSubscriptions();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}