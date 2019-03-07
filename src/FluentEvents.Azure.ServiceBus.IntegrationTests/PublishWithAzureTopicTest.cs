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

            TestEventArgs testEventArgs = null;
            m_TestEventsContext.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) => { testEventArgs = args; };
            });

            var entity = new TestEntity();
            m_TestEventsContext.Attach(entity, m_EventsScope);

            entity.RaiseEvent("");

            var waitMilliseconds = 20000;
            var checksCount = 20;
            for (var i = 0; i < checksCount; i++)
            {
                await Task.Delay(waitMilliseconds / checksCount);

                if (testEventArgs != null)
                    break;
            }

            Assert.That(testEventArgs, Is.Not.Null);
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
