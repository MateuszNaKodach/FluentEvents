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
    public class QueueEventSenderTests
    {
        private const string ConnectionString = Constants.ValidConnectionString;

        private Mock<ILogger<QueueEventSender>> _loggerMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<IQueueClientFactory> _queueClientFactoryMock;
        private Mock<IQueueClient> _queueClientMock;
        private QueueEventSenderConfig _queueEventSenderConfig;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<QueueEventSender>>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _queueClientFactoryMock = new Mock<IQueueClientFactory>(MockBehavior.Strict);
            _queueClientMock = new Mock<IQueueClient>(MockBehavior.Strict);
            _queueEventSenderConfig = new QueueEventSenderConfig
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

            var azureServiceBusQueueEventSender = new QueueEventSender(
                _loggerMock.Object,
                _eventsSerializationServiceMock.Object,
                Options.Create(_queueEventSenderConfig),
                _queueClientFactoryMock.Object
            );
        }
    }
}