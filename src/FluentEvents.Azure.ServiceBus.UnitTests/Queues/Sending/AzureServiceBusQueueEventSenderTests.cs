using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Azure.ServiceBus.Queues.Sending;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues.Sending
{
    [TestFixture]
    public class AzureServiceBusQueueEventSenderTests
    {
        private const string ConnectionString = Constants.ValidConnectionString;

        private Mock<ILogger<AzureServiceBusQueueEventSender>> _loggerMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<IQueueClientFactory> _queueClientFactoryMock;
        private Mock<IQueueClient> _queueClientMock;
        private AzureServiceBusQueueEventSenderConfig _azureServiceBusQueueEventSenderConfig;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<AzureServiceBusQueueEventSender>>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _queueClientFactoryMock = new Mock<IQueueClientFactory>(MockBehavior.Strict);
            _queueClientMock = new Mock<IQueueClient>(MockBehavior.Strict);
            _azureServiceBusQueueEventSenderConfig = new AzureServiceBusQueueEventSenderConfig
            {
                SendConnectionString = ConnectionString
            };
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _queueClientFactoryMock.Verify();
            _queueClientMock.Verify();
        }

        [Test]
        public void Ctor_ShouldGetQueueClientFromFactory()
        {
            _queueClientFactoryMock
                .Setup(x => x.GetNew(ConnectionString))
                .Returns(_queueClientMock.Object)
                .Verifiable();

            var azureServiceBusQueueEventSender = new AzureServiceBusQueueEventSender(
                _loggerMock.Object,
                _eventsSerializationServiceMock.Object,
                Options.Create(_azureServiceBusQueueEventSenderConfig),
                _queueClientFactoryMock.Object
            );
        }
    }
}