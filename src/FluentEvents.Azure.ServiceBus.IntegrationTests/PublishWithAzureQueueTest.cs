using System;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Queues;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.IntegrationTests
{
    [TestFixture]
    public class PublishWithAzureQueueTest
    {
        private TestEventsContext _testEventsContext;
        private IServiceProvider _serviceProvider;
        private EventsScope _eventsScope;

        [SetUp]
        public void SetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<PublishWithAzureQueueTest>()
                .Build();

            if (string.IsNullOrEmpty(configuration["azureQueueSender:sendConnectionString"]))
                Assert.Ignore("Azure Service Bus queue settings not found in user secrets.");

            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.UseAzureQueueEventReceiver(configuration.GetSection("azureQueueReceiver"));
                options.UseAzureQueueEventSender(configuration.GetSection("azureQueueSender"));
            });

            _serviceProvider = services.BuildServiceProvider();

            _testEventsContext = _serviceProvider.GetRequiredService<TestEventsContext>();
            _eventsScope = _serviceProvider.CreateScope().ServiceProvider.GetService<EventsScope>();
        }

        [Test]
        public async Task EventShouldBePublishedWithAzureServiceBusQueue()
        {
            await _testEventsContext.StartEventReceiversAsync();

            object receivedSender = null;
            TestEventArgs receivedEventArgs = null;
            _testEventsContext.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    receivedSender = sender;
                    receivedEventArgs = args;
                };
            });

            TestUtils.AttachAndRaiseEvent(_testEventsContext, _eventsScope);

            await Watcher.WaitUntilAsync(() => receivedEventArgs != null);

            TestUtils.AssertThatEventIsPublishedProperly(receivedSender, receivedEventArgs);
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsWatched()
                    .ThenIsPublishedToGlobalSubscriptions(x => x.WithAzureQueue());
            }
        }
    }
}
