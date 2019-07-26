using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using FluentEvents.ServiceProviders;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class SubscriptionsWithInterfaceTest
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
        public void SubscriptionsWithEventInterfaces_ShouldHandleEventsImplementingTheseInterfaces()
        {
            var subscribingService = _appServiceProvider.GetRequiredService<SubscribingService>();
            var testEventsContext = _appServiceProvider.GetRequiredService<TestEventsContext>();
            var eventsScope = _appServiceProvider.CreateScope().ServiceProvider.GetRequiredService<EventsScope>();

            TestUtils.AttachAndRaiseEvent(testEventsContext, eventsScope);
            
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.ITestEvents)).With.One.Items);
            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.ITestEvents)).With.One.Items.TypeOf<TestEvent>());
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingSubscriptions(ISubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, ITestEvent>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }
    }
}
