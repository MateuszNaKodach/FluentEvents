using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class PipelineAndSubscriptionInheritanceTest
    {
        private IServiceProvider _appServiceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddSingleton<SubscribingService>();
            services.AddEventsContext<TestEventsContext>(options => { });
            _appServiceProvider = services.BuildServiceProvider();
        }

        [Test]
        public void WithBothPipelineAndSubscriptionsAreConfiguredWithInherited_ShouldPublishInheritingEvents()
        {
            var subscribingService = _appServiceProvider.GetRequiredService<SubscribingService>();
            var testEventsContext = _appServiceProvider.GetRequiredService<TestEventsContext>();

            TestUtils.AttachAndRaiseEvent(testEventsContext);
            
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.BaseTestEvents)).With.One.Items);
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.BaseTestEvents)).With.One.Items.TypeOf<TestEvent>());
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.TestEvents)).Empty);
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEventBase>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEventBase>()
                    .IsPiped()
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext(
                EventsContextsRoot eventsContextsRoot,
                EventsContextOptions options,
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(eventsContextsRoot, options, scopedAppServiceProvider)
            {
            }
        }
    }
}
