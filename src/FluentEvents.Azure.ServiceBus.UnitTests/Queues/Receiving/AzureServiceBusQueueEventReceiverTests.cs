using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Azure.ServiceBus.Queues.Receiving;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues.Receiving
{
    [TestFixture]
    public class AzureServiceBusQueueEventReceiverTests
    {
        private const string ReceiveConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=read;SharedAccessKey=0;EntityPath=0";

        private AzureServiceBusQueueEventReceiverConfig _config;

        private Mock<ILogger<AzureServiceBusQueueEventReceiver>> _loggerMock;
        private Mock<IPublishingService> _publishingServiceMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<IQueueClientFactory> _queueClientFactoryMock;
        private Mock<IQueueClient> _queueClientMock;

        private AzureServiceBusQueueEventReceiver _azureServiceBusQueueEventReceiver;

        [SetUp]
        public void SetUp()
        {
            _config = new AzureServiceBusQueueEventReceiverConfig
            {
                ReceiveConnectionString = ReceiveConnectionString,
                MaxConcurrentMessages = 10
            };
            _loggerMock = new Mock<ILogger<AzureServiceBusQueueEventReceiver>>(MockBehavior.Strict);
            _publishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _queueClientFactoryMock = new Mock<IQueueClientFactory>(MockBehavior.Strict);
            _queueClientMock = new Mock<IQueueClient>(MockBehavior.Strict);

            _azureServiceBusQueueEventReceiver = new AzureServiceBusQueueEventReceiver(
                _queueClientFactoryMock.Object,
                _loggerMock.Object,
                _eventsSerializationServiceMock.Object,
                _publishingServiceMock.Object,
                Options.Create(_config)
            );
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _publishingServiceMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _queueClientFactoryMock.Verify();
            _queueClientMock.Verify();
        }

        [Test]
        public async Task CreateReceiverClient_ShouldReturnQueueClient()
        {
            var cts = new CancellationTokenSource();

            _queueClientFactoryMock
                .Setup(x => x.GetNew(_config.ReceiveConnectionString))
                .Returns(_queueClientMock.Object)
                .Verifiable();

            await _azureServiceBusQueueEventReceiver.CreateReceiverClientAsync(cts.Token);
        }
    }
}