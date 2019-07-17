using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.IntegrationTests
{
    [TestFixture]
    public class PublishWithAzureTopicTest
    {
        private TestEventsContext _testEventsContext;
        private IServiceProvider _serviceProvider;
        private EventReceiversHostedService _eventReceiversHostedService;

        [SetUp]
        public void SetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .AddUserSecrets<PublishWithAzureTopicTest>()
                .Build();

            if (string.IsNullOrEmpty(configuration["azureTopicSender:sendConnectionString"]))
                Assert.Ignore("Azure Service Bus settings not found in user secrets.");

            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.UseAzureTopicEventReceiver(configuration.GetSection("azureTopicReceiver"));
                options.UseAzureTopicEventSender(configuration.GetSection("azureTopicSender"));
            });
            services.AddSingleton<SubscribingService>();
            
            _serviceProvider = services.BuildServiceProvider();

            _testEventsContext = _serviceProvider.GetRequiredService<TestEventsContext>();
            _eventReceiversHostedService = (EventReceiversHostedService) _serviceProvider
                .GetRequiredService<IHostedService>();
        }

        [Test]
        public async Task EventShouldBePublishedWithAzureServiceBusTopic()
        {
            await _eventReceiversHostedService.StartAsync(CancellationToken.None);

            var subscribingService = _serviceProvider.GetRequiredService<SubscribingService>();

            TestUtils.AttachAndRaiseEvent(_testEventsContext);

            await Watcher.WaitUntilAsync(() => subscribingService.TestEvents.Any());

            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.TestEvents)).With.One.Items);

            TestUtils.AssertThatEventIsPublishedProperly(subscribingService.TestEvents.FirstOrDefault());
        }

        private class TestEventsContext : EventsContext
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
                    .ThenIsPublishedToGlobalSubscriptions(x => x.WithAzureTopic());
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
