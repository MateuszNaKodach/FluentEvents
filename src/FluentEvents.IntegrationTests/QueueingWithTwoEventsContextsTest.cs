using System;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class QueueingWithTwoEventsContextsTest
    {
        private const string QueueName = nameof(QueueName);

        private IServiceProvider _appServiceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddSingleton<SubscribingService>();
            services.AddEventsContext<TestEventsContext1>(options => { });
            services.AddEventsContext<TestEventsContext2>(options => { });
            _appServiceProvider = services.BuildServiceProvider();
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_ShouldProcessOnlyTestEventsContext1Events()
        {
            var subscribingService = _appServiceProvider.GetRequiredService<SubscribingService>();
            var testEventsContext1 = _appServiceProvider.GetRequiredService<TestEventsContext1>();
            var testEventsContext2 = _appServiceProvider.GetRequiredService<TestEventsContext2>();
            var eventsScope = _appServiceProvider.CreateScope().ServiceProvider.GetRequiredService<EventsScope>();

            TestUtils.AttachAndRaiseEvent(testEventsContext1, eventsScope);
            TestUtils.AttachAndRaiseEvent(testEventsContext2, eventsScope);

            await testEventsContext1.ProcessQueuedEventsAsync(eventsScope, QueueName);

            TestUtils.AssertThatEventIsPublishedProperly(subscribingService.TestEvents.FirstOrDefault());

            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.TestEvents)).With.One.Items);
        }

        private class TestEventsContext1 : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEvent>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsQueuedTo(QueueName)
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext1(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }

        private class TestEventsContext2 : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEvent>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsQueuedTo(QueueName)
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext2(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }
    }
}
