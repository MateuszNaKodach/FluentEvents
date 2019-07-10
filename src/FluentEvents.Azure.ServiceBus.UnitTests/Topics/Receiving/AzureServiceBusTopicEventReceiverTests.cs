using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Topics.Receiving;
using FluentEvents.Azure.ServiceBus.Topics.Subscribing;
using FluentEvents.Pipelines;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Topics.Receiving
{
    [TestFixture]
    public class AzureServiceBusTopicEventReceiverTests
    {
        private const string TopicPath = "TopicPath";
        private const string ManagementConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=management;SharedAccessKey=0;EntityPath=0";
        private const string ReceiveConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=read;SharedAccessKey=0;EntityPath=0";
        private const string SubscriptionName = "SubscriptionName";

        private AzureServiceBusTopicEventReceiverConfig _config;

        private Mock<ILogger<AzureServiceBusTopicEventReceiver>> _loggerMock;
        private Mock<IPublishingService> _publishingServiceMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<ITopicSubscriptionsService> _topicSubscriptionsServiceMock;
        private Mock<ISubscriptionClientFactory> _subscriptionClientFactoryMock;
        private Mock<ISubscriptionClient> _subscriptionClientMock;

        private AzureServiceBusTopicEventReceiver _azureServiceBusTopicEventReceiver;

        [SetUp]
        public void SetUp()
        {
            _config = new AzureServiceBusTopicEventReceiverConfig
            {
                ReceiveConnectionString = ReceiveConnectionString,
                ManagementConnectionString = ManagementConnectionString,
                SubscriptionsAutoDeleteOnIdleTimeout = TimeSpan.FromDays(1),
                TopicPath = TopicPath,
                SubscriptionNameGenerator = () => SubscriptionName,
                MaxConcurrentMessages = 10
            };
            _loggerMock = new Mock<ILogger<AzureServiceBusTopicEventReceiver>>(MockBehavior.Strict);
            _publishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _topicSubscriptionsServiceMock = new Mock<ITopicSubscriptionsService>(MockBehavior.Strict);
            _subscriptionClientFactoryMock = new Mock<ISubscriptionClientFactory>(MockBehavior.Strict);
            _subscriptionClientMock = new Mock<ISubscriptionClient>(MockBehavior.Strict);

            _azureServiceBusTopicEventReceiver = new AzureServiceBusTopicEventReceiver(
                _loggerMock.Object,
                Options.Create(_config),
                _publishingServiceMock.Object,
                _eventsSerializationServiceMock.Object,
                _topicSubscriptionsServiceMock.Object,
                _subscriptionClientFactoryMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _publishingServiceMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _topicSubscriptionsServiceMock.Verify();
            _subscriptionClientFactoryMock.Verify();
            _subscriptionClientMock.Verify();
        }

        [Test]
        public async Task CreateReceiverClient_ShouldReturnSubscriptionClient()
        {
            var cts = new CancellationTokenSource();

            _topicSubscriptionsServiceMock
                .Setup(x => x.CreateSubscriptionAsync(
                    _config.ManagementConnectionString,
                    SubscriptionName,
                    _config.TopicPath,
                    _config.SubscriptionsAutoDeleteOnIdleTimeout,
                    cts.Token
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _subscriptionClientFactoryMock
                .Setup(x => x.GetNew(_config.ReceiveConnectionString, SubscriptionName))
                .Returns(_subscriptionClientMock.Object)
                .Verifiable();

            await _azureServiceBusTopicEventReceiver.CreateReceiverClientAsync(cts.Token);
        }
    }
}
