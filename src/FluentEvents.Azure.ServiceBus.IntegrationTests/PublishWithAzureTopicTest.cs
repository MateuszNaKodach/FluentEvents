using System;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.IntegrationTests
{
    [TestFixture]
    public class PublishWithAzureTopicTest
    {
        private TestEventsContext m_TestEventsContext;
        private IServiceProvider m_ServiceProvider;
        private EventsScope m_EventsScope;

        [SetUp]
        public void SetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<PublishWithAzureTopicTest>()
                .Build();

            if (string.IsNullOrEmpty(configuration["azureTopicSender:connectionString"]))
                Assert.Ignore("Azure Service Bus settings not found in user secrets.");

            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.UseAzureTopicEventReceiver(configuration.GetSection("azureTopicReceiver"));
                options.UseAzureTopicEventSender(configuration.GetSection("azureTopicSender"));
            });

            m_ServiceProvider = services.BuildServiceProvider();

            m_TestEventsContext = m_ServiceProvider.GetRequiredService<TestEventsContext>();
            m_EventsScope = m_ServiceProvider.CreateScope().ServiceProvider.GetService<EventsScope>();
        }

        [Test]
        public async Task EventShouldBePublishedWithAzureServiceBusTopic()
        {
            await m_TestEventsContext.StartEventReceivers();

            object receivedSender = null;
            TestEventArgs receivedEventArgs = null;
            m_TestEventsContext.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    receivedSender = sender;
                    receivedEventArgs = args;
                };
            });

            TestUtils.AttachAndRaiseEvent(m_TestEventsContext, m_EventsScope);

            await Watcher.WaitUntilAsync(() => receivedEventArgs != null);

            TestUtils.AssertThatEventIsPublishedProperly(receivedSender, receivedEventArgs);
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsForwardedToPipeline()
                    .ThenIsPublishedToGlobalSubscriptions(x => x.WithAzureTopic());
            }
        }
    }
}
