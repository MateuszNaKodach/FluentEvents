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
        private readonly string m_TestValue = "TestValue";

        private TestEventsContext m_Context;
        private TestEntity m_Entity;
        private SubscribingService m_ScopedSubscribingService;
        private SubscribingService m_SingletonSubscribingService;
        private IServiceProvider m_ServiceProvider;
        private EventsScope m_Scope;

        private ProjectedTestEntity m_ProjectedTestEntity;
        private ProjectedEventArgs m_ProjectedEventArgs;
        private TestEntity m_TestEntity;
        private TestEventArgs m_TestEventArgs;

        private void SetUpContext(TestRunParameters testRunParameters)
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options => { });

            services.AddSingleton(testRunParameters);

            services.AddScoped<ScopedSubscribingService>();
            services.AddSingleton<SingletonSubscribingService>();
            m_ServiceProvider = services.BuildServiceProvider();

            m_Entity = new TestEntity();

            var serviceScope = m_ServiceProvider.CreateScope();
            m_ScopedSubscribingService = serviceScope.ServiceProvider.GetRequiredService<ScopedSubscribingService>();
            m_SingletonSubscribingService = serviceScope.ServiceProvider.GetRequiredService<SingletonSubscribingService>();
            m_Context = m_ServiceProvider.GetRequiredService<TestEventsContext>();
            m_Scope = serviceScope.ServiceProvider.GetRequiredService<EventsScope>();

            m_Context.Attach(m_Entity, m_Scope);
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

            if (isAsync)
                await m_Entity.RaiseAsyncEvent(m_TestValue);
            else
                m_Entity.RaiseEvent(m_TestValue);

            if (isQueued)
                await m_Context.ProcessQueuedEventsAsync(m_Scope);

            var service = publicationType == PublicationType.Scoped
                ? m_ScopedSubscribingService
                : m_SingletonSubscribingService;

            if (publicationType == PublicationType.GlobalWithManualSubscription)
            {
                Assert.That(isProjected ? (object)m_ProjectedTestEntity : m_TestEntity, Is.Not.Null);
                Assert.That(isProjected ? (object)m_ProjectedEventArgs : m_TestEventArgs, Is.Not.Null);
            }
            else
            {
                Assert.That(service,
                    isProjected
                        ? Has.Property(nameof(SubscribingService.ProjectedEventArgs)).Not.Null
                        : Has.Property(nameof(SubscribingService.EventArgs)).Not.Null
                );
            }
        }

        private void SetUpManualGlobalSubscription(
            bool isAsync, 
            bool isProjected
        )
        {
            if (isProjected)
                m_Context.SubscribeGloballyTo<ProjectedTestEntity>(x =>
                {
                    if (isAsync)
                        x.AsyncTest += (sender, args) =>
                        {
                            m_ProjectedTestEntity = (ProjectedTestEntity) sender;
                            m_ProjectedEventArgs = args;
                            return Task.CompletedTask;
                        };
                    else
                        x.Test += (sender, args) =>
                        {
                            m_ProjectedTestEntity = (ProjectedTestEntity) sender;
                            m_ProjectedEventArgs = args;
                        };
                });
            else
                m_Context.SubscribeGloballyTo<TestEntity>(x =>
                {
                    if (isAsync)
                        x.AsyncTest += (sender, args) =>
                        {
                            m_TestEntity = (TestEntity) sender;
                            m_TestEventArgs = args;
                            return Task.CompletedTask;
                        };
                    else
                        x.Test += (sender, args) =>
                        {
                            m_TestEntity = (TestEntity) sender;
                            m_TestEventArgs = args;
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
            private readonly TestRunParameters m_Parameters;

            public TestEventsContext(TestRunParameters parameters)
            {
                m_Parameters = parameters;
            }

            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                if (m_Parameters.PublicationType == PublicationType.GlobalWithManualSubscription)
                    return;

                subscriptionsBuilder
                    .Service<ScopedSubscribingService>()
                    .HasScopedSubscription<TestEntity>((service, entity) => service.Subscribe(entity))
                    .HasScopedSubscription<TestEntity>((service, entity) => service.AsyncSubscribe(entity));

                subscriptionsBuilder
                    .Service<SingletonSubscribingService>()
                    .HasGlobalSubscription<TestEntity>((service, entity) => service.Subscribe(entity))
                    .HasGlobalSubscription<TestEntity>((service, entity) => service.AsyncSubscribe(entity));

                if (!m_Parameters.IsProjected)
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
                var eventFieldName = m_Parameters.IsAsync ? nameof(TestEntity.AsyncTest) : nameof(TestEntity.Test);

                var pipelineConfigurator = pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(eventFieldName)
                    .IsForwardedToPipeline();

                if (m_Parameters.IsFiltered)
                    pipelineConfigurator.ThenIsFiltered((sender, args) => true);

                if (m_Parameters.IsProjected)
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
                if (m_Parameters.IsQueued)
                    pipelineConfigurator.ThenIsQueuedTo("DefaultQueue");

                switch (m_Parameters.PublicationType)
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