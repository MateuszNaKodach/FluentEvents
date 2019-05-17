using System;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Filters;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;
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
        private EventsScope _scope;

        private ProjectedTestEntity _projectedTestEntity;
        private ProjectedEventArgs _projectedEventArgs;
        private TestEntity _testEntity;
        private TestEventArgs _testEventArgs;

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
            _context = _serviceProvider.GetRequiredService<TestEventsContext>();
            _scope = serviceScope.ServiceProvider.GetRequiredService<EventsScope>();

            _context.Attach(_entity, _scope);
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

            if (publicationType == PublicationType.GlobalWithManualSubscription)
                SetUpManualGlobalSubscription(isAsync, isProjected);

            await RaiseEvent(isAsync);

            if (isQueued)
                await _context.ProcessQueuedEventsAsync(_scope);

            var subscribingService = publicationType == PublicationType.Scoped
                ? _scopedSubscribingService
                : _singletonSubscribingService;

            if (publicationType == PublicationType.GlobalWithManualSubscription)
            {
                Assert.That(isProjected ? (object)_projectedTestEntity : _testEntity, Is.Not.Null);
                Assert.That(isProjected ? (object)_projectedEventArgs : _testEventArgs, Is.Not.Null);
            }
            else
            {
                Assert.That(subscribingService,
                    isProjected
                        ? Has.Property(nameof(SubscribingService.ProjectedEventArgs)).Not.Null
                        : Has.Property(nameof(SubscribingService.EventArgs)).Not.Null
                );
            }
        }

        private async Task RaiseEvent(bool isAsync)
        {
            if (isAsync)
                await _entity.RaiseAsyncEvent(_testValue);
            else
                _entity.RaiseEvent(_testValue);
        }

        private void SetUpManualGlobalSubscription(
            bool isAsync, 
            bool isProjected
        )
        {
            if (isProjected)
                _context.SubscribeGloballyTo<ProjectedTestEntity>(x =>
                {
                    if (isAsync)
                        x.AsyncTest += (sender, args) =>
                        {
                            _projectedTestEntity = (ProjectedTestEntity) sender;
                            _projectedEventArgs = args;
                            return Task.CompletedTask;
                        };
                    else
                        x.Test += (sender, args) =>
                        {
                            _projectedTestEntity = (ProjectedTestEntity) sender;
                            _projectedEventArgs = args;
                        };
                });
            else
                _context.SubscribeGloballyTo<TestEntity>(x =>
                {
                    if (isAsync)
                        x.AsyncTest += (sender, args) =>
                        {
                            _testEntity = (TestEntity) sender;
                            _testEventArgs = args;
                            return Task.CompletedTask;
                        };
                    else
                        x.Test += (sender, args) =>
                        {
                            _testEntity = (TestEntity) sender;
                            _testEventArgs = args;
                        };
                });
        }

        public enum PublicationType
        {
            GlobalWithServiceSubscription,
            GlobalWithManualSubscription,
            Scoped
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

            public TestEventsContext(TestRunParameters parameters)
            {
                _parameters = parameters;
            }

            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                if (_parameters.PublicationType == PublicationType.GlobalWithManualSubscription)
                    return;

                subscriptionsBuilder
                    .Service<ScopedSubscribingService>()
                    .HasScopedSubscription<TestEntity>((service, entity) => service.Subscribe(entity))
                    .HasScopedSubscription<TestEntity>((service, entity) => service.AsyncSubscribe(entity));

                subscriptionsBuilder
                    .Service<SingletonSubscribingService>()
                    .HasGlobalSubscription<TestEntity>((service, entity) => service.Subscribe(entity))
                    .HasGlobalSubscription<TestEntity>((service, entity) => service.AsyncSubscribe(entity));

                if (!_parameters.IsProjected)
                    return;

                subscriptionsBuilder
                    .Service<ScopedSubscribingService>()
                    .HasScopedSubscription<ProjectedTestEntity>((service, entity) => service.Subscribe(entity))
                    .HasScopedSubscription<ProjectedTestEntity>((service, entity) => service.AsyncSubscribe(entity));

                subscriptionsBuilder
                    .Service<SingletonSubscribingService>()
                    .HasGlobalSubscription<ProjectedTestEntity>((service, entity) => service.Subscribe(entity))
                    .HasGlobalSubscription<ProjectedTestEntity>((service, entity) => service.AsyncSubscribe(entity));
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                var eventFieldName = _parameters.IsAsync ? nameof(TestEntity.AsyncTest) : nameof(TestEntity.Test);

                var pipelineConfigurator = pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(eventFieldName)
                    .IsWatched();

                if (_parameters.IsFiltered)
                    pipelineConfigurator.ThenIsFiltered((sender, args) => true);

                if (_parameters.IsProjected)
                {
                    var pipelineConfiguratorWithProjection = pipelineConfigurator.ThenIsProjected(
                        sender => new ProjectedTestEntity(),
                        args => new ProjectedEventArgs()
                    );

                    ContinueConfiguration(pipelineConfiguratorWithProjection);
                }
                else
                {
                    ContinueConfiguration(pipelineConfigurator);
                }
            }

            private void ContinueConfiguration<TSender, TArgs>(
                EventPipelineConfigurator<TSender, TArgs> pipelineConfigurator
            )
                where TSender : class
                where TArgs : class
            {
                if (_parameters.IsQueued)
                    pipelineConfigurator.ThenIsQueuedTo("DefaultQueue");

                switch (_parameters.PublicationType)
                {
                    case PublicationType.GlobalWithServiceSubscription:
                    case PublicationType.GlobalWithManualSubscription:
                        pipelineConfigurator.ThenIsPublishedToGlobalSubscriptions();
                        break;
                    case PublicationType.Scoped:
                        pipelineConfigurator.ThenIsPublishedToScopedSubscriptions();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}