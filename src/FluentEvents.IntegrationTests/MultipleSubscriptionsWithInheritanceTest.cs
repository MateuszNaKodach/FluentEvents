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
    public class MultipleSubscriptionsWithInheritanceTest
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
        public void SubscriptionsWithInheritedEvents_ShouldHandleInheritingEvents()
        {
            var subscribingService = _appServiceProvider.GetRequiredService<SubscribingService>();
            var testEventsContext = _appServiceProvider.GetRequiredService<TestEventsContext>();

            TestUtils.AttachAndRaiseEvent(testEventsContext);
            
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.BaseTestEvents)).With.One.Items);
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.BaseTestEvents)).With.One.Items.TypeOf<TestEvent>());
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.TestEvents)).With.One.Items);
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.TestEvents)).With.One.Items.TypeOf<TestEvent>());
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEventBase>()
                    .HasGlobalSubscription();

                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEvent>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext(
                EventsContextOptions options, 
                IAppServiceProvider appServiceProvider, 
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(options, appServiceProvider, scopedAppServiceProvider)
            {
            }
        }
    }
}
